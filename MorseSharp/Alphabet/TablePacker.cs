using System;
using System.Collections.Generic;

namespace MorseSharp.Alphabet;

/// <summary>
/// Turns a list of entries into the lookup tables an alphabet uses.
/// </summary>
/// <remarks>
/// <para>
/// This file is compiled into both MorseSharp and the source generator. The generator runs inside the compiler and
/// therefore targets netstandard2.0, which it cannot share with the library any other way. Keeping one copy is what
/// guarantees that a table baked at build time is bit-for-bit what the runtime would have produced, so tuning
/// <see cref="Hash"/> can never silently desynchronise the two.
/// </para>
/// <para>
/// That sharing is also why nothing here uses spans, <c>BitOperations</c>, collection expressions or the modern throw
/// helpers. It is plain arithmetic over arrays, which costs nothing.
/// </para>
/// </remarks>
internal static class TablePacker
{
    /// <summary>The longest pattern an alphabet may contain, in symbols.</summary>
    public const int MaxSymbols = 8;

    /// <summary>Exclusive upper bound of valid tree codes.</summary>
    public const int CodeLimit = 1 << (MaxSymbols + 1);

    /// <summary>Tree code of the empty pattern; used as the code of the word separator.</summary>
    public const int WordSpaceCode = 1;

    /// <summary>Multiplier for the Fibonacci hash, 2^32 divided by the golden ratio.</summary>
    private const uint HashMultiplier = 0x9E3779B1u;

    /// <summary>
    /// Packs <paramref name="entries"/> into lookup tables. Primaries are inserted first and claim their pattern for
    /// decoding; aliases follow and only claim a pattern no primary took, so the result never depends on whether
    /// the primary or the alias was declared first.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when two primaries share a pattern, or when one character is mapped to two patterns.
    /// </exception>
    public static PackedTables Pack(string name, IReadOnlyList<MorseEntry> entries)
    {
        int slots = 0;
        for (int i = 0; i < entries.Count; i++)
            slots += CountNonAsciiSlots(entries[i].Character);

        PackedTables tables = new PackedTables(ComputeHashedSize(slots));

        // The word separator is reserved, so a space can never be remapped.
        InsertChar(tables, name, ' ', WordSpaceCode);
        tables.Decode[WordSpaceCode] = ' ';
        tables.DecodableCount = 1;

        for (int pass = 0; pass < 2; pass++)
        {
            bool aliasPass = pass == 1;
            for (int i = 0; i < entries.Count; i++)
            {
                MorseEntry entry = entries[i];
                if (entry.IsAlias != aliasPass)
                    continue;

                int code = ParseCode(entry.Pattern);
                if (!aliasPass)
                {
                    if (tables.Decode[code] != '\0')
                    {
                        throw new InvalidOperationException(
                            name + ": pattern '" + entry.Pattern + "' is used by both '" + tables.Decode[code] +
                            "' and '" + entry.Character + "'. Add one of them with AddAlias instead.");
                    }

                    tables.Decode[code] = entry.Character;
                    tables.DecodableCount++;
                }
                else if (tables.Decode[code] == '\0')
                {
                    tables.Decode[code] = entry.Character;
                    tables.DecodableCount++;
                }

                InsertChar(tables, name, entry.Character, code);
            }
        }

        return tables;
    }

    /// <summary>Converts a dot/dash string to its tree code.</summary>
    /// <exception cref="ArgumentException">Thrown for an empty or over-long pattern, or one containing another symbol.</exception>
    public static int ParseCode(string pattern)
    {
        if (string.IsNullOrEmpty(pattern) || pattern.Length > MaxSymbols)
            throw new ArgumentException("Pattern '" + pattern + "' must have between 1 and " + MaxSymbols + " symbols.", nameof(pattern));

        int code = 1;
        for (int i = 0; i < pattern.Length; i++)
        {
            char c = pattern[i];
            if (c == '.')
                code = code << 1;
            else if (c == '-')
                code = (code << 1) | 1;
            else
                throw new ArgumentException("Pattern '" + pattern + "' contains '" + c + "'; only '.' and '-' are allowed.", nameof(pattern));
        }

        return code;
    }

    /// <summary>
    /// Chooses the hash table size for a given number of non-ASCII slots: a power of two keeping the load factor at
    /// or below one half, or zero when the alphabet is pure ASCII and needs no table at all.
    /// </summary>
    public static int ComputeHashedSize(int nonAsciiSlots)
    {
        if (nonAsciiSlots <= 0)
            return 0;

        int size = 4;
        while (size < nonAsciiSlots * 2)
            size <<= 1;
        return size;
    }

    /// <summary>
    /// The right shift that reduces a hash to <paramref name="hashedSize"/> slots. Fibonacci hashing carries its
    /// quality in the high bits, so the table index is the top bits rather than a masked-off low slice.
    /// </summary>
    public static int HashShiftFor(int hashedSize)
    {
        if (hashedSize <= 0)
            return 32;

        int bits = 0;
        while ((1 << bits) < hashedSize)
            bits++;
        return 32 - bits;
    }

    /// <summary>Reduces a character to a table slot.</summary>
    public static int Hash(uint key, int hashShift) => (int)((key * HashMultiplier) >> hashShift);

    /// <summary>
    /// Counts the hash slots a character will occupy, matching what <see cref="InsertChar"/> inserts. Used to size
    /// the table before anything is written to it, so the count must never be lower than the real one.
    /// </summary>
    public static int CountNonAsciiSlots(char ch)
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

    private static void InsertChar(PackedTables tables, string name, char ch, int code)
    {
        InsertExact(tables, name, ch, code);

        char lower = char.ToLowerInvariant(ch);
        if (lower != ch)
            InsertExact(tables, name, lower, code);

        char upper = char.ToUpperInvariant(ch);
        if (upper != ch)
            InsertExact(tables, name, upper, code);
    }

    private static void InsertExact(PackedTables tables, string name, char ch, int code)
    {
        if (ch < 128)
        {
            ushort existingAscii = tables.Ascii[ch];
            if (existingAscii != 0 && existingAscii != code)
                throw new InvalidOperationException(name + ": character '" + ch + "' is mapped to two different patterns.");
            tables.Ascii[ch] = (ushort)code;
            return;
        }

        uint[] table = tables.Hashed;
        uint key = ch;
        uint entry = ((uint)code << 16) | key;
        int mask = table.Length - 1;
        int index = Hash(key, tables.HashShift);

        for (int probe = 1; probe <= table.Length; probe++)
        {
            uint existing = table[index];
            if (existing == 0)
            {
                table[index] = entry;
                if (probe > tables.MaxProbeLength)
                    tables.MaxProbeLength = probe;
                return;
            }

            if ((ushort)existing == key)
            {
                if (existing != entry)
                    throw new InvalidOperationException(name + ": character '" + ch + "' is mapped to two different patterns.");
                return;
            }

            index = (index + 1) & mask;
        }

        throw new InvalidOperationException(name + ": hash table is full.");
    }
}
