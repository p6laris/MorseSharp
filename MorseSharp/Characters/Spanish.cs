namespace MorseSharp.Characters;

/*
 * Sourced from https://morsedecoder.com/.
 * If you notice an incorrect character please open an issue at https://github.com/p6laris/MorseSharp.
 */
internal static class SpanishCharacters
{
    private static readonly (char Char, string Code)[] Letters =
    [
        ('Á', ".--.-"), ('É', "..-.."), ('Ñ', "--.--"), ('Ó', "---."),
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
        ('Í', ".."),  // same as I
        ('Ù', "..-"), // same as U
        ('Ü', "..-"), // same as U
    ];
}
