using System.Buffers.Binary;

namespace MorseSharp;

/// <summary>
/// An immutable set of character-to-Morse mappings, shared by every thread that uses it.
/// </summary>
/// <remarks>
/// <para>
/// Obtain one either from a built-in language, through <see cref="Morse.ForLanguage"/>, or by building your own
/// with <see cref="MorseAlphabetBuilder"/> and passing it to <see cref="Morse.ForAlphabet"/>. Conversion itself
/// never looks characters up through <see cref="Name"/> or <see cref="Characters"/>; those exist purely for callers
/// who want to inspect or list an alphabet's mappings.
/// </para>
/// <para>
/// A Morse pattern is stored as a <b>tree code</b>: start at 1 and, for every symbol, shift left and add 1 for a dash
/// or 0 for a dot. The code therefore encodes both the symbols and the length (the position of the leading 1 bit).
/// Patterns of up to <see cref="MaxSymbols"/> symbols fit in 9 bits, so decoding is a single array index.
/// </para>
/// <para>
/// Encoding uses a direct 128-entry table for ASCII and an open-addressing hash table for everything else, sized to
/// the alphabet rather than to a fixed capacity. The tables come from <see cref="Alphabet.TablePacker"/>, packed at
/// run time for a custom alphabet or at build time for a built-in one, which arrives ready made via
/// <see cref="FromBlobs"/>.
/// </para>
/// </remarks>
public sealed class MorseAlphabet
{
    /// <summary>The longest pattern an alphabet may contain, in symbols.</summary>
    public const int MaxSymbols = TablePacker.MaxSymbols;

    /// <summary>Exclusive upper bound of valid tree codes.</summary>
    internal const int CodeLimit = TablePacker.CodeLimit;

    /// <summary>Tree code of the empty pattern; used as the code of the word separator.</summary>
    internal const int WordSpaceCode = TablePacker.WordSpaceCode;

    private readonly ushort[] _ascii;
    private readonly uint[] _hashed;  // (code << 16) | key, 0 = empty; may be zero-length
    private readonly int _hashShift;  // reduces the hash to the table size by keeping its top bits
    private readonly int _hashMask;   // _hashed.Length - 1
    private readonly char[] _decode;  // '\0' = no character

    // Prosigns sit beside the decode table rather than in it: that table holds one character per
    // entry and cannot represent a multi-letter signal. Owners come first, so decoding stops at
    // _decodableProsigns.
    private readonly string[] _prosignNames;
    private readonly int[] _prosignCodes;
    private readonly int _decodableProsigns;

    private readonly Func<MorseEntry[]>? _entriesFactory;
    private MorseEntry[]? _entries;
    private MorseCharacterEntry[]? _characters;

    /// <summary>The name of this alphabet, used in error messages.</summary>
    public string Name { get; }

    /// <summary>The characters this alphabet maps, and the pattern each is keyed as.</summary>
    public IReadOnlyList<MorseCharacterEntry> Characters => _characters ??= BuildCharacters();

    /// <summary>Returns the built-in alphabet for a language.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="language"/> is not a defined value.</exception>
    public static MorseAlphabet ForLanguage(Language language) => Alphabets.For(language);

    /// <summary>Longest probe sequence in the non-ASCII hash table (diagnostics/tests).</summary>
    internal int MaxProbeLength { get; }

    /// <summary>Number of distinct patterns that decode to a character (word separator included).</summary>
    internal int DecodableCount { get; }

    /// <summary>Slots allocated for the non-ASCII hash table; zero for a pure ASCII alphabet (diagnostics/tests).</summary>
    internal int HashedSlots => _hashed.Length;

    /// <summary>
    /// The entries this alphabet was packed from, so it can be extended. A built-in alphabet rebuilds them on demand
    /// rather than at startup, because only <see cref="MorseAlphabetBuilder.From(Language)"/> ever needs them.
    /// </summary>
    internal MorseEntry[] Entries => _entries ??= _entriesFactory!();

    private MorseCharacterEntry[] BuildCharacters()
    {
        MorseEntry[] entries = Entries;
        MorseCharacterEntry[] characters = new MorseCharacterEntry[entries.Length];
        for (int i = 0; i < entries.Length; i++)
            characters[i] = new MorseCharacterEntry(entries[i].Character, entries[i].Pattern, entries[i].IsAlias);

        return characters;
    }

    private MorseAlphabet(
        string name,
        ushort[] ascii,
        uint[] hashed,
        char[] decode,
        int hashShift,
        int maxProbeLength,
        int decodableCount,
        MorseEntry[]? entries,
        Func<MorseEntry[]>? entriesFactory,
        string[] prosignNames,
        int[] prosignCodes,
        int decodableProsigns)
    {
        Name = name;
        _ascii = ascii;
        _hashed = hashed;
        _decode = decode;
        _hashShift = hashShift;
        _hashMask = hashed.Length - 1;
        MaxProbeLength = maxProbeLength;
        DecodableCount = decodableCount;
        _entries = entries;
        _entriesFactory = entriesFactory;
        _prosignNames = prosignNames;
        _prosignCodes = prosignCodes;
        _decodableProsigns = decodableProsigns;
    }

    /// <summary>
    /// The most characters one decoded token can occupy, used to size the decode buffer. One for a plain character,
    /// or the widest bracketed prosign name plus its two brackets.
    /// </summary>
    internal int MaxDecodedTokenLength
    {
        get
        {
            int widest = 1;
            for (int i = 0; i < _decodableProsigns; i++)
            {
                int length = _prosignNames[i].Length + 2;
                if (length > widest)
                    widest = length;
            }

            return widest;
        }
    }

    /// <summary>How many prosigns this alphabet defines.</summary>
    internal int ProsignCount => _prosignNames.Length;

    /// <summary>The prosigns this alphabet was built from, so it can be extended.</summary>
    internal MorseProsign[] Prosigns
    {
        get
        {
            MorseProsign[] prosigns = new MorseProsign[_prosignNames.Length];
            Span<char> buffer = stackalloc char[MaxSymbols];
            for (int i = 0; i < prosigns.Length; i++)
            {
                int position = 0;
                WriteCode(buffer, ref position, _prosignCodes[i]);
                prosigns[i] = new MorseProsign(_prosignNames[i], buffer[..position].ToString(), i >= _decodableProsigns);
            }

            return prosigns;
        }
    }

    /// <summary>
    /// Looks up the tree code of a prosign, given its letters without brackets.
    /// </summary>
    /// <remarks>
    /// A linear scan, because an alphabet defines a handful of prosigns at most and this only runs when the text
    /// actually contains a bracketed token.
    /// </remarks>
    internal bool TryGetProsignCode(ReadOnlySpan<char> name, out int code)
    {
        for (int i = 0; i < _prosignNames.Length; i++)
        {
            if (name.Equals(_prosignNames[i].AsSpan(), StringComparison.OrdinalIgnoreCase))
            {
                code = _prosignCodes[i];
                return true;
            }
        }

        code = 0;
        return false;
    }

    /// <summary>Returns the prosign that owns a tree code, if one does.</summary>
    internal bool TryGetProsignName(int code, out string name)
    {
        for (int i = 0; i < _decodableProsigns; i++)
        {
            if (_prosignCodes[i] == code)
            {
                name = _prosignNames[i];
                return true;
            }
        }

        name = string.Empty;
        return false;
    }

    /// <summary>Packs entries into an alphabet at run time. Used by <see cref="MorseAlphabetBuilder"/>.</summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when two primaries share a pattern, or when one character is mapped to two patterns.
    /// </exception>
    internal static MorseAlphabet Pack(string name, List<MorseEntry> entries) => Pack(name, entries, []);

    /// <summary>Packs entries and prosigns into an alphabet at run time. Used by <see cref="MorseAlphabetBuilder"/>.</summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when two primaries share a pattern, or when one character is mapped to two patterns.
    /// </exception>
    internal static MorseAlphabet Pack(string name, List<MorseEntry> entries, List<MorseProsign> prosigns)
    {
        PackedTables tables = TablePacker.Pack(name, entries, prosigns);
        return new MorseAlphabet(
            name, tables.Ascii, tables.Hashed, tables.Decode, tables.HashShift,
            tables.MaxProbeLength, tables.DecodableCount, entries.ToArray(), entriesFactory: null,
            tables.ProsignNames, tables.ProsignCodes, tables.DecodableProsigns);
    }

    /// <summary>
    /// Builds an alphabet from tables already packed at compile time by the source generator, skipping the hashing
    /// and probing entirely. The blobs live in the assembly's data section, so this only copies them into arrays.
    /// </summary>
    /// <param name="name">The alphabet name.</param>
    /// <param name="ascii">The ASCII table, 128 little-endian <see cref="ushort"/> values.</param>
    /// <param name="hashed">The non-ASCII table as little-endian <see cref="uint"/> values; may be empty.</param>
    /// <param name="decode">The decode table, <see cref="CodeLimit"/> little-endian <see cref="char"/> values.</param>
    /// <param name="maxProbeLength">Longest probe run recorded while packing.</param>
    /// <param name="decodableCount">How many patterns decode to a character.</param>
    /// <param name="entriesFactory">Rebuilds the entry list on demand, for extending the alphabet.</param>
    /// <param name="prosignNames">Prosign names, those owning their pattern first.</param>
    /// <param name="prosignCodes">Tree codes matching <paramref name="prosignNames"/> position for position.</param>
    /// <param name="decodableProsigns">How many prosigns own their pattern, and so appear when decoding.</param>
    internal static MorseAlphabet FromBlobs(
        string name,
        ReadOnlySpan<byte> ascii,
        ReadOnlySpan<byte> hashed,
        ReadOnlySpan<byte> decode,
        int maxProbeLength,
        int decodableCount,
        Func<MorseEntry[]> entriesFactory,
        string[] prosignNames,
        int[] prosignCodes,
        int decodableProsigns)
    {
        // These copies are deliberate. A ReadOnlySpan cannot be a field of a class, so going zero-copy would mean
        // holding raw pointers: into the data section here, and into pinned arrays for a runtime-built alphabet.
        // Measured, that trades ~100ns and ~1.4KB saved once per language against custom alphabets allocating 5x
        // slower on the pinned object heap and pressuring gen2. Copying is the cheaper end of that trade.
        ushort[] asciiTable = MemoryMarshal.Cast<byte, ushort>(ascii).ToArray();
        uint[] hashedTable = MemoryMarshal.Cast<byte, uint>(hashed).ToArray();
        char[] decodeTable = MemoryMarshal.Cast<byte, char>(decode).ToArray();

        // The blobs are emitted little-endian so they can be embedded verbatim.
        if (!BitConverter.IsLittleEndian)
        {
            BinaryPrimitives.ReverseEndianness(asciiTable, asciiTable);
            BinaryPrimitives.ReverseEndianness(hashedTable, hashedTable);
            Span<ushort> decodeAsUInt16 = MemoryMarshal.Cast<char, ushort>(decodeTable.AsSpan());
            BinaryPrimitives.ReverseEndianness(decodeAsUInt16, decodeAsUInt16);
        }

        return new MorseAlphabet(
            name, asciiTable, hashedTable, decodeTable, TablePacker.HashShiftFor(hashedTable.Length),
            maxProbeLength, decodableCount, entries: null, entriesFactory,
            prosignNames, prosignCodes, decodableProsigns);
    }

    /// <summary>Converts a dot/dash string to its tree code.</summary>
    /// <exception cref="ArgumentException">Thrown for an empty or over-long pattern, or one containing another symbol.</exception>
    internal static int ParseCode(string pattern) => TablePacker.ParseCode(pattern);

    /// <summary>Chooses the hash table size for a number of non-ASCII slots.</summary>
    internal static int ComputeHashedSize(int nonAsciiSlots) => TablePacker.ComputeHashedSize(nonAsciiSlots);

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
        int index = TablePacker.Hash(key, _hashShift);
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
