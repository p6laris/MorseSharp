namespace MorseSharp.Characters;

/*
 * Kurdish (Sorani, Arabic script). The mapping is derived from the Arabic and Persian tables;
 * see KurdishToMorse.md in the repository for the reasoning behind each letter.
 */
internal static class KurdishCharacters
{
    private static readonly (char Char, string Code)[] Letters =
    [
        ('ا', ".-"),    ('ب', "-..."),  ('پ', ".--."),  ('ت', "-"),     ('ج', ".---"),   ('چ', "---."),
        ('ح', "...."),  ('خ', "-..-"),  ('د', "-.."),   ('ر', "-.-"),   ('ڕ', ".-."),    ('ز', "--.."),
        ('ژ', "--."),   ('س', "..."),   ('ش', "----"),  ('ع', "---"),   ('غ', "..--"),   ('ف', "..-."),
        ('ڤ', "..-.."), ('ق', "...---"), ('ک', "-.-.."), ('گ', "--.-"),  ('ل', ".-.."),   ('ڵ', "...-"),
        ('م', "--"),    ('ن', "-."),    ('ه', "-.-."),  ('ە', "."),     ('و', ".--"),    ('ۆ', ".-.-"),
        ('ی', ".."),    ('ێ', "..-"),   ('ئ', "..-..-"),
    ];

    /// <summary>Punctuation shared by both Kurdish scripts.</summary>
    internal static readonly (char Char, string Code)[] Punctuation =
    [
        ('.', ".-.-.-."), ('،', "--..--"), ('؟', "..--.."), (':', "---..."), ('-', "-....-"),
        ('/', "-..-."),   ('\'', ".----."), ('"', ".-..-."), ('؛', "-.-.-."), ('_', "..--.-"),
        ('+', ".-.-."),   ('=', "-...-"),  (')', "-.--.-"), ('(', "-.--."),  ('$', "...-..-"),
        ('¿', "..-.-"),   ('¡', "--...-"), ('&', ".-..."),  ('@', ".--.-."), ('!', "-.-.--"),
    ];

    /// <summary>Entries whose pattern is unique; every one of them round-trips through encode and decode.</summary>
    internal static readonly (char Char, string Code)[][] Primaries =
    [
        Letters,
        SharedCharacters.ArabicIndicDigits,
        Punctuation,
    ];

    /// <summary>Encode-only entries that share a pattern with a primary entry.</summary>
    internal static readonly (char Char, string Code)[] Aliases =
    [
        ('*', "-..-"), // ITU multiplication sign, same as خ
    ];
}
