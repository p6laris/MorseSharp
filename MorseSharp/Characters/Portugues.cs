namespace MorseSharp.Characters;

/*
 * Sourced from https://morsedecoder.com/.
 * If you notice an incorrect character please open an issue at https://github.com/p6laris/MorseSharp.
 */
internal static class PortuguesCharacters
{
    private static readonly (char Char, string Code)[] Letters =
    [
        ('Ã', ".--.-"), ('Ç', "-.-.."), ('É', "..-.."), ('Ó', "---."), ('Ù', "..--"),
    ];

    /// <summary>Entries whose pattern is unique; every one of them round-trips through encode and decode.</summary>
    internal static readonly (char Char, string Code)[][] Primaries =
    [
        SharedCharacters.LatinLetters,
        Letters,
        SharedCharacters.Digits,
        SharedCharacters.Punctuation,
    ];

    /// <summary>Encode-only entries that share a pattern with a primary entry.</summary>
    internal static readonly (char Char, string Code)[] Aliases =
    [
        ('Á', ".--.-"), // same as Ã
        ('À', ".--.-"), // same as Ã
        ('Â', ".--.-"), // same as Ã
        ('Ê', "-..-."), // same as /
        ('Ô', "---."),  // same as Ó
    ];
}
