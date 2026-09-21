using System.Collections.Generic;
using MorseSharp.Alphabet;

namespace MorseSharp.Generators;

/// <summary>
/// One <c>.morse</c> file after parsing: the entries it declared, or the problems that stopped it parsing.
/// </summary>
internal sealed class ParsedAlphabet
{
    /// <summary>Creates a parse result.</summary>
    public ParsedAlphabet(
        string name,
        string filePath,
        List<MorseEntry> entries,
        List<MorseProsign> prosigns,
        List<AlphabetError> errors)
    {
        Name = name;
        FilePath = filePath;
        Entries = entries;
        Prosigns = prosigns;
        Errors = errors;
    }

    /// <summary>The alphabet name, taken from the file name, which must match a <c>Language</c> member.</summary>
    public string Name { get; }

    /// <summary>Path of the file this came from, for diagnostics.</summary>
    public string FilePath { get; }

    /// <summary>The characters, in declaration order, primaries and aliases together.</summary>
    public List<MorseEntry> Entries { get; }

    /// <summary>The prosigns, in declaration order.</summary>
    public List<MorseProsign> Prosigns { get; }

    /// <summary>Problems found while parsing. When this is non-empty nothing is emitted for the file.</summary>
    public List<AlphabetError> Errors { get; }
}
