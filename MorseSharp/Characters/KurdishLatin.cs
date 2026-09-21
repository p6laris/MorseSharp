namespace MorseSharp.Characters;

/*
 * Kurdish (Hawar Latin alphabet). Each letter carries the code of its Arabic-script counterpart in Kurdish.cs.
 * See KurdishToMorse.md in the repository.
 */
internal static class KurdishLatinCharacters
{
    private static readonly (char Char, string Code)[] Letters =
    [
        ('A', ".-"),    ('B', "-..."),  ('P', ".--."),  ('T', "-"),     ('C', ".---"),   ('Ç', "---."),
        ('Ü', "...."),  ('X', "-..-"),  ('D', "-.."),   ('R', "-.-"),   ('Ř', ".-."),    ('Z', "--.."),
        ('J', "--."),   ('S', "..."),   ('Ş', "----"),  ('W', "---"),   ('Y', "..--"),   ('F', "..-."),
        ('V', "..-.."), ('Ň', "...---"), ('K', "-.-.."), ('G', "--.-"),  ('L', ".-.."),   ('Ł', "...-"),
        ('M', "--"),    ('N', "-."),    ('H', "-.-."),  ('E', "."),     ('U', ".--"),    ('Û', ".--.--"),
        ('O', ".-.-"),  ('Î', ".."),    ('Ê', "..-"),   ('I', "..-..-"),
    ];

    /// <summary>Entries whose pattern is unique; every one of them round-trips through encode and decode.</summary>
    internal static readonly (char Char, string Code)[][] Primaries =
    [
        Letters,
        SharedCharacters.Digits,
        KurdishCharacters.Punctuation,
    ];

    /// <summary>Encode-only entries that share a pattern with a primary entry.</summary>
    internal static readonly (char Char, string Code)[] Aliases =
    [
        ('*', "-..-"), // ITU multiplication sign, same as X
    ];

    internal static MorseAlphabet Build() => MorseAlphabet.Build(Language.KurdishLatin, Primaries, Aliases);
}
