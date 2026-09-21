namespace MorseSharp.Characters;

/*
 * Sourced from https://morsedecoder.com/.
 * If you notice an incorrect character please open an issue at https://github.com/p6laris/MorseSharp.
 */
internal static class FrenchCharacters
{
    private static readonly (char Char, string Code)[] Letters =
    [
        ('À', ".--.-"), ('Æ', ".-.-"), ('Ç', "-.-.."), ('È', ".-..-"),
        ('Ë', "..-.."), ('Ï', "-..--"), ('Ô', "---."), ('Ü', "..--"),
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
        ('Â', ".--.-"), // same as À
        ('É', "..-.."), // same as Ë
        ('Ê', "-..-."), // same as /
        ('Ù', "..--"),  // same as Ü
    ];

    internal static MorseAlphabet Build() => MorseAlphabet.Build(Language.French, Primaries, Aliases);
}
