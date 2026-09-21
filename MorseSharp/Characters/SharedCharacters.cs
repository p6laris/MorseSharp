namespace MorseSharp.Characters;

/// <summary>
/// Entry groups shared by several languages. The word separator (space) is added by the builder and is not listed.
/// </summary>
internal static class SharedCharacters
{
    /// <summary>ITU international Morse, letters A-Z.</summary>
    internal static readonly (char Char, string Code)[] LatinLetters =
    [
        ('A', ".-"),   ('B', "-..."), ('C', "-.-."), ('D', "-.."),  ('E', "."),    ('F', "..-."),
        ('G', "--."),  ('H', "...."), ('I', ".."),   ('J', ".---"), ('K', "-.-"),  ('L', ".-.."),
        ('M', "--"),   ('N', "-."),   ('O', "---"),  ('P', ".--."), ('Q', "--.-"), ('R', ".-."),
        ('S', "..."),  ('T', "-"),    ('U', "..-"),  ('V', "...-"), ('W', ".--"),  ('X', "-..-"),
        ('Y', "-.--"), ('Z', "--.."),
    ];

    /// <summary>ITU digits 0-9.</summary>
    internal static readonly (char Char, string Code)[] Digits =
    [
        ('1', ".----"), ('2', "..---"), ('3', "...--"), ('4', "....-"), ('5', "....."),
        ('6', "-...."), ('7', "--..."), ('8', "---.."), ('9', "----."), ('0', "-----"),
    ];

    /// <summary>Arabic-Indic digits ٠-٩ with the ITU digit codes.</summary>
    internal static readonly (char Char, string Code)[] ArabicIndicDigits =
    [
        ('١', ".----"), ('٢', "..---"), ('٣', "...--"), ('٤', "....-"), ('٥', "....."),
        ('٦', "-...."), ('٧', "--..."), ('٨', "---.."), ('٩', "----."), ('٠', "-----"),
    ];

    /// <summary>ITU punctuation plus the common extensions ($, ¿, ¡, &amp;, !).</summary>
    internal static readonly (char Char, string Code)[] Punctuation =
    [
        ('.', ".-.-.-"), (',', "--..--"), ('?', "..--.."), (';', "-.-.-."), (':', "---..."),
        ('/', "-..-."),  ('\'', ".----."), ('"', ".-..-."), ('_', "..--.-"), ('+', ".-.-."),
        ('-', "-....-"), ('=', "-...-"),  ('(', "-.--."),  (')', "-.--.-"), ('$', "...-..-"),
        ('¿', "..-.-"),  ('¡', "--...-"), ('&', ".-..."),  ('@', ".--.-."), ('!', "-.-.--"),
    ];

    /// <summary>No entries.</summary>
    internal static readonly (char Char, string Code)[] None = [];
}
