namespace MorseSharp;

/// <summary>
/// An immutable set of character-to-Morse mappings, shared by every thread that uses it.
/// </summary>
/// <remarks>
/// <para>
/// Obtain one either from a built-in language, through <see cref="Morse.ForLanguage"/>, or by building your own
/// with <see cref="MorseAlphabetBuilder"/> and passing it to <see cref="Morse.ForAlphabet"/>. The type is opaque:
/// it exposes only its <see cref="Name"/>, because callers never look characters up themselves.
/// </para>
/// <para>
/// A Morse pattern is stored as a <b>tree code</b>: start at 1 and, for every symbol, shift left and add 1 for a dash
/// or 0 for a dot. The code therefore encodes both the symbols and the length (the position of the leading 1 bit).
/// Patterns of up to <see cref="MaxSymbols"/> symbols fit in 9 bits, so decoding is a single array index.
/// </para>
/// <para>
/// Encoding uses a direct 128-entry table for ASCII and an open-addressing hash table for everything else. That
/// second table is sized to the alphabet actually being built rather than to a fixed capacity, so a small alphabet
/// stays small and a large custom one still fits. Both letter cases are inserted while packing, so lookups never
/// need a case conversion.
/// </para>
/// </remarks>
public sealed class MorseAlphabet
{
    /// <summary>The longest pattern an alphabet may contain, in symbols.</summary>
    public const int MaxSymbols = 8;

    /// <summary>Exclusive upper bound of valid tree codes.</summary>
    internal const int CodeLimit = 1 << (MaxSymbols + 1);

    /// <summary>Tree code of the empty pattern; used as the code of the word separator.</summary>
    internal const int WordSpaceCode = 1;

    private readonly ushort[] _ascii;
    private readonly uint[] _hashed;  // (code << 16) | key, 0 = empty; may be zero-length
    private readonly int _hashShift;  // 32 - log2(_hashed.Length), so the top bits of the hash pick the slot
    private readonly int _hashMask;   // _hashed.Length - 1
    private readonly char[] _decode;  // '\0' = no character

    /// <summary>The name of this alphabet, used in error messages.</summary>
    public string Name { get; }

    /// <summary>The entries this alphabet was packed from, in order, so it can be extended or inspected.</summary>
    internal MorseEntry[] Entries { get; }

    /// <summary>Longest probe sequence in the non-ASCII hash table (diagnostics/tests).</summary>
    internal int MaxProbeLength { get; private set; }

    /// <summary>Slots allocated for the non-ASCII hash table; zero for a pure ASCII alphabet (diagnostics/tests).</summary>
    internal int HashedSlots => _hashed.Length;

    /// <summary>Number of distinct patterns that decode to a character (word separator included).</summary>
    internal int DecodableCount { get; private set; }

    private MorseAlphabet(string name, int hashedSize, MorseEntry[] entries)
    {
        Name = name;
        Entries = entries;
        _ascii = new ushort[128];
        _decode = new char[CodeLimit];

        if (hashedSize == 0)
        {
            _hashed = [];
            _hashMask = 0;
            _hashShift = 32;
        }
        else
        {
            _hashed = new uint[hashedSize];
            _hashMask = hashedSize - 1;
            _hashShift = 32 - BitOperations.Log2((uint)hashedSize);
        }
    }

    /// <summary>
    /// Packs <paramref name="entries"/> into lookup tables. Primaries are inserted first and claim their pattern for
    /// decoding; aliases follow and only claim a pattern no primary took, so the result never depends on whether
    /// <c>Add</c> or <c>AddAlias</c> was called first.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when two primaries share a pattern, or when one character is mapped to two patterns.
    /// </exception>
    internal static MorseAlphabet Pack(string name, List<MorseEntry> entries)
    {
        int slots = 0;
        foreach (MorseEntry entry in entries)
            slots += CountNonAsciiSlots(entry.Character);

        MorseAlphabet alphabet = new(name, ComputeHashedSize(slots), entries.ToArray());

        // The word separator is reserved, so a space can never be remapped.
        alphabet.InsertChar(' ', WordSpaceCode);
        alphabet._decode[WordSpaceCode] = ' ';
        alphabet.DecodableCount = 1;

        foreach (MorseEntry entry in entries)
        {
            if (entry.IsAlias)
                continue;

            int code = ParseCode(entry.Pattern);
            if (alphabet._decode[code] != '\0')
                throw new InvalidOperationException(
                    $"{name}: pattern '{entry.Pattern}' is used by both '{alphabet._decode[code]}' and '{entry.Character}'. Add one of them with AddAlias instead.");

            alphabet._decode[code] = entry.Character;
            alphabet.DecodableCount++;
            alphabet.InsertChar(entry.Character, code);
        }

        foreach (MorseEntry entry in entries)
        {
            if (!entry.IsAlias)
                continue;

            int code = ParseCode(entry.Pattern);
            if (alphabet._decode[code] == '\0')
            {
                alphabet._decode[code] = entry.Character;
                alphabet.DecodableCount++;
            }
            alphabet.InsertChar(entry.Character, code);
        }

        return alphabet;
    }

    /// <summary>
    /// Chooses the hash table size for a given number of non-ASCII slots: a power of two keeping the load factor at
    /// or below one half, or zero when the alphabet is pure ASCII and needs no table at all.
    /// </summary>
    internal static int ComputeHashedSize(int nonAsciiSlots)
    {
        if (nonAsciiSlots == 0)
            return 0;

        int size = 4;
        while (size < nonAsciiSlots * 2)
            size <<= 1;
        return size;
    }

    /// <summary>
    /// Counts the hash slots a character will occupy, matching what <see cref="InsertChar"/> inserts. Used to size
    /// the table before anything is written to it, so the count must never be lower than the real one.
    /// </summary>
    private static int CountNonAsciiSlots(char ch)
    {
        char lower = char.ToLowerInvariant(ch);
        char upper = char.ToUpperInvariant(ch);

        int count = ch >= 128 ? 1 : 0;
        if (lower != ch && lower >= 128)
            count++;
        if (upper != ch && upper != lower && upper >= 128)
            count++;
        return count;
    }

    /// <summary>Converts a dot/dash string to its tree code.</summary>
    /// <exception cref="ArgumentException">Thrown for an empty or over-long pattern, or one containing another symbol.</exception>
    internal static int ParseCode(ReadOnlySpan<char> pattern)
    {
        if (pattern.Length is 0 or > MaxSymbols)
            throw new ArgumentException($"Pattern '{pattern}' must have between 1 and {MaxSymbols} symbols.", nameof(pattern));

        int code = 1;
        foreach (char c in pattern)
        {
            code = c switch
            {
                '.' => code << 1,
                '-' => (code << 1) | 1,
                _ => throw new ArgumentException($"Pattern '{pattern}' contains '{c}'; only '.' and '-' are allowed.", nameof(pattern)),
            };
        }
        return code;
    }

    /// <summary>Number of symbols in a tree code (0 for the word separator).</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int SymbolCount(int code) => BitOperations.Log2((uint)code);

    /// <summary>
    /// Writes the dots and dashes of <paramref name="code"/> to <paramref name="destination"/> starting at <paramref name="position"/>.
    /// The word separator is written as <c>/</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void WriteCode(Span<char> destination, ref int position, int code)
    {
        if (code == WordSpaceCode)
        {
            destination[position++] = '/';
            return;
        }

        int length = SymbolCount(code);
        for (int i = length - 1; i >= 0; i--)
        {
            // '-' is 0x2D and '.' is 0x2E, so a set bit subtracts one from '.'.
            destination[position++] = (char)('.' - ((code >> i) & 1));
        }
    }

    /// <summary>Number of characters <see cref="WriteCode"/> produces for a code.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int WrittenLength(int code) => code == WordSpaceCode ? 1 : SymbolCount(code);

    private void InsertChar(char ch, int code)
    {
        InsertExact(ch, code);

        char lower = char.ToLowerInvariant(ch);
        if (lower != ch)
            InsertExact(lower, code);

        char upper = char.ToUpperInvariant(ch);
        if (upper != ch)
            InsertExact(upper, code);
    }

    private void InsertExact(char ch, int code)
    {
        if (ch < 128)
        {
            ref ushort slot = ref _ascii[ch];
            if (slot != 0 && slot != code)
                throw new InvalidOperationException($"{Name}: character '{ch}' is mapped to two different patterns.");
            slot = (ushort)code;
            return;
        }

        uint[] table = _hashed;
        uint key = ch;
        uint entry = ((uint)code << 16) | key;
        int index = Hash(key);

        for (int probe = 1; probe <= table.Length; probe++)
        {
            ref uint existing = ref table[index];
            if (existing == 0)
            {
                existing = entry;
                if (probe > MaxProbeLength)
                    MaxProbeLength = probe;
                return;
            }
            if ((ushort)existing == key)
            {
                if (existing != entry)
                    throw new InvalidOperationException($"{Name}: character '{ch}' is mapped to two different patterns.");
                return;
            }
            index = (index + 1) & _hashMask;
        }

        throw new InvalidOperationException($"{Name}: hash table is full.");
    }

    /// <summary>
    /// Fibonacci hash reduced to the table size by keeping its top bits, which are the well distributed ones.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Hash(uint key) => (int)((key * 0x9E3779B1u) >> _hashShift);

    /// <summary>
    /// Looks up the tree code of a character.
    /// </summary>
    /// <returns><c>true</c> when the character exists in this alphabet.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool TryGetCode(char ch, out int code)
    {
        if (ch < 128)
        {
            code = _ascii[ch];
            return code != 0;
        }

        return TryGetCodeHashed(ch, out code);
    }

    private bool TryGetCodeHashed(char ch, out int code)
    {
        uint[] table = _hashed;
        if (table.Length == 0)
        {
            code = 0;
            return false;
        }

        uint key = ch;
        int index = Hash(key);
        while (true)
        {
            uint entry = table[index];
            if (entry == 0)
            {
                code = 0;
                return false;
            }
            if ((ushort)entry == key)
            {
                code = (int)(entry >> 16);
                return true;
            }
            index = (index + 1) & _hashMask;
        }
    }

    /// <summary>
    /// Returns the character for a tree code, or <c>'\0'</c> when none exists.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal char Decode(int code) => (uint)code < CodeLimit ? _decode[code] : '\0';
}
