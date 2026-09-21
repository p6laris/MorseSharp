namespace MorseSharp.Characters;

/*
 * Sourced from https://morsedecoder.com/.
 * If you notice an incorrect character please open an issue at https://github.com/p6laris/MorseSharp.
 */
internal static class ArabicCharacters
{
    private static readonly (char Char, string Code)[] Letters =
    [
        ('ا', ".-"),   ('ب', "-..."), ('ت', "-"),    ('ث', "-.-."), ('ج', ".---"),  ('ح', "...."),
        ('خ', "---"),  ('د', "-.."),  ('ذ', "--.."), ('ر', ".-."),  ('ز', "---."),  ('س', "..."),
        ('ش', "----"), ('ص', "-..-"), ('ض', "...-"), ('ط', "..-"),  ('ظ', "-.--"),  ('ع', ".-.-"),
        ('غ', "--."),  ('ف', "..-."), ('ق', "--.-"), ('ك', "-.-"),  ('ل', ".-.."),  ('م', "--"),
        ('ن', "-."),   ('ه', "..-.."), ('و', ".--"),  ('ي', ".."),   ('ء', "."),
    ];

    /// <summary>Arabic punctuation marks plus the ITU symbols.</summary>
    internal static readonly (char Char, string Code)[] Punctuation =
    [
        ('.', ".-.-.-"), ('،', "--..--"), ('؟', "..--.."), ('؛', "-.-.-."), (':', "---..."),
        ('/', "-..-."),  ('‘', ".----."), ('"', ".-..-."), ('_', "..--.-"), ('+', ".-.-."),
        ('-', "-....-"), ('=', "-...-"),  (')', "-.--.-"), ('(', "-.--."),  ('$', "...-..-"),
        ('¿', "..-.-"),  ('¡', "--...-"), ('&', ".-..."),  ('@', ".--.-."), ('!', "-.-.--"),
    ];

    /// <summary>Entries whose pattern is unique; every one of them round-trips through encode and decode.</summary>
    internal static readonly (char Char, string Code)[][] Primaries =
    [
        Letters,
        SharedCharacters.ArabicIndicDigits,
        Punctuation,
    ];

    /// <summary>Encode-only entries that share a pattern with a primary entry.</summary>
    internal static readonly (char Char, string Code)[] Aliases = SharedCharacters.None;

    internal static MorseAlphabet Build() => MorseAlphabet.Build(Language.Arabic, Primaries, Aliases);
}
