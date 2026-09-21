using System;

namespace MorseSharp.Alphabet;

/// <summary>
/// The three lookup tables an alphabet is made of, in the exact layout <see cref="MorseAlphabet"/> reads at runtime
/// and the source generator serialises at build time.
/// </summary>
/// <remarks>
/// Shared verbatim with the source generator, so it must stay netstandard2.0-compatible.
/// </remarks>
internal sealed class PackedTables
{
    /// <summary>Creates empty tables sized for <paramref name="hashedSize"/> non-ASCII slots.</summary>
    public PackedTables(int hashedSize)
    {
        Ascii = new ushort[128];
        Hashed = hashedSize == 0 ? Array.Empty<uint>() : new uint[hashedSize];
        Decode = new char[TablePacker.CodeLimit];
        HashShift = TablePacker.HashShiftFor(hashedSize);
    }

    /// <summary>Direct lookup for characters below U+0080, indexed by the character itself. 0 means absent.</summary>
    public ushort[] Ascii { get; }

    /// <summary>Open-addressed table for the rest, holding <c>(code &lt;&lt; 16) | key</c>. 0 means empty.</summary>
    public uint[] Hashed { get; }

    /// <summary>Tree code to character. <c>'\0'</c> means the pattern decodes to nothing.</summary>
    public char[] Decode { get; }

    /// <summary>Right shift that reduces the hash to the table size, keeping its top bits.</summary>
    public int HashShift { get; }

    /// <summary>Longest probe run seen while inserting.</summary>
    public int MaxProbeLength { get; set; }

    /// <summary>How many distinct patterns decode to a character, the word separator included.</summary>
    public int DecodableCount { get; set; }
}
