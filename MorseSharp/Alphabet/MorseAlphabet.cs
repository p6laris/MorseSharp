namespace MorseSharp.Alphabet;

/// <summary>
/// Immutable, shared lookup tables for one language.
/// </summary>
/// <remarks>
/// <para>
/// A Morse pattern is stored as a <b>tree code</b>: start at 1 and, for every symbol, shift left and add 1 for a dash
/// or 0 for a dot. The code therefore encodes both the symbols and the length (the position of the leading 1 bit).
/// Patterns of up to <see cref="MaxSymbols"/> symbols fit in 9 bits, so decoding is a single array index.
/// </para>
/// <para>
/// Encoding uses a direct 128-entry table for ASCII and a 256-slot open-addressing hash table for everything else.
/// Both letter cases are inserted at build time, so lookups never need a case conversion.
/// </para>
/// </remarks>
internal sealed class MorseAlphabet
{
    /// <summary>Longest pattern that can be represented (8 symbols, enough for every prosign in use).</summary>
    public const int MaxSymbols = 8;

    /// <summary>Exclusive upper bound of valid tree codes.</summary>
    public const int CodeLimit = 1 << (MaxSymbols + 1);

    /// <summary>Tree code of the empty pattern; used as the code of the word separator.</summary>
    public const int WordSpaceCode = 1;

    private const int HashSize = 256;
    private const int HashMask = HashSize - 1;

    private readonly ushort[] _ascii = new ushort[128];
    private readonly uint[] _hashed = new uint[HashSize];  // (code << 16) | key, 0 = empty
    private readonly char[] _decode = new char[CodeLimit]; // '\0' = no character

    /// <summary>The language these tables describe.</summary>
    public Language Language { get; }

    /// <summary>Longest probe sequence in the non-ASCII hash table (diagnostics/tests).</summary>
    internal int MaxProbeLength { get; private set; }

    /// <summary>Number of distinct patterns that decode to a character (word separator included).</summary>
    internal int DecodableCount { get; private set; }

    private MorseAlphabet(Language language) => Language = language;

    /// <summary>
    /// Builds an alphabet. Entries are added in order; when two characters share a pattern the first one wins
    /// for decoding and later ones become encode-only aliases.
    /// </summary>
    /// <param name="language">The language.</param>
    /// <param name="groups">Groups of (character, pattern) entries whose patterns must be unique across all groups.</param>
    /// <param name="aliases">Encode-only entries that may share a pattern with an earlier entry.</param>
    internal static MorseAlphabet Build(Language language, ReadOnlySpan<(char Char, string Code)[]> groups, (char Char, string Code)[] aliases)
    {
        MorseAlphabet alphabet = new(language);
        alphabet.Insert(' ', WordSpaceCode);
        alphabet._decode[WordSpaceCode] = ' ';
        alphabet.DecodableCount = 1;

        foreach ((char Char, string Code)[] group in groups)
        {
            foreach ((char ch, string pattern) in group)
            {
                int code = ParseCode(pattern);
                if (alphabet._decode[code] != '\0')
                    throw new InvalidOperationException($"{language}: pattern '{pattern}' is defined for both '{alphabet._decode[code]}' and '{ch}'. Move one of them to the alias list.");
                alphabet._decode[code] = ch;
                alphabet.DecodableCount++;
                alphabet.Insert(ch, code);
            }
        }

        foreach ((char ch, string pattern) in aliases)
        {
            int code = ParseCode(pattern);
            if (alphabet._decode[code] == '\0')
            {
                alphabet._decode[code] = ch;
                alphabet.DecodableCount++;
            }
            alphabet.Insert(ch, code);
        }

        return alphabet;
    }

    /// <summary>Converts a dot/dash string to its tree code.</summary>
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
    public static int SymbolCount(int code) => BitOperations.Log2((uint)code);

    /// <summary>
    /// Writes the dots and dashes of <paramref name="code"/> to <paramref name="destination"/> starting at <paramref name="position"/>.
    /// The word separator is written as <c>/</c>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteCode(Span<char> destination, ref int position, int code)
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
    public static int WrittenLength(int code) => code == WordSpaceCode ? 1 : SymbolCount(code);

    private void Insert(char ch, int code)
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
                throw new InvalidOperationException($"{Language}: character '{ch}' is mapped to two different patterns.");
            slot = (ushort)code;
            return;
        }

        uint key = ch;
        uint entry = ((uint)code << 16) | key;
        int index = Hash(key);
        for (int probe = 1; probe <= HashSize; probe++)
        {
            ref uint slot = ref _hashed[index];
            if (slot == 0)
            {
                slot = entry;
                if (probe > MaxProbeLength)
                    MaxProbeLength = probe;
                return;
            }
            if ((ushort)slot == key)
            {
                if (slot != entry)
                    throw new InvalidOperationException($"{Language}: character '{ch}' is mapped to two different patterns.");
                return;
            }
            index = (index + 1) & HashMask;
        }
        throw new InvalidOperationException($"{Language}: hash table is full.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Hash(uint key) => (int)((key * 0x9E3779B1u) >> 24);

    /// <summary>
    /// Looks up the tree code of a character.
    /// </summary>
    /// <returns><c>true</c> when the character exists in this alphabet.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetCode(char ch, out int code)
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
        uint key = ch;
        uint[] table = _hashed;
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
            index = (index + 1) & HashMask;
        }
    }

    /// <summary>
    /// Returns the character for a tree code, or <c>'\0'</c> when none exists.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public char Decode(int code) => (uint)code < CodeLimit ? _decode[code] : '\0';
}
