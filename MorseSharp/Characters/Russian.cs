namespace MorseSharp.Characters;

/*
 * Sourced from https://en.wikipedia.org/wiki/Russian_Morse_code.
 * If you notice an incorrect character please open an issue at https://github.com/p6laris/MorseSharp.
 */
internal static class RussianCharacters
{
    private static readonly (char Char, string Code)[] Letters =
    [
        ('А', ".-"),   ('Б', "-..."), ('В', ".--"),  ('Г', "--."),   ('Д', "-.."),  ('Е', "."),
        ('Ж', "...-"), ('З', "--.."), ('И', ".."),   ('Й', ".---"),  ('К', "-.-"),  ('Л', ".-.."),
        ('М', "--"),   ('Н', "-."),   ('О', "---"),  ('П', ".--."),  ('Р', ".-."),  ('С', "..."),
        ('Т', "-"),    ('У', "..-"),  ('Ф', "..-."), ('Х', "...."),  ('Ц', "-.-."), ('Ч', "---."),
        ('Ш', "----"), ('Щ', "--.-"), ('Ъ', "-..-"), ('Ы', "-.--"),  ('Э', "..-.."), ('Ю', "..--"),
        ('Я', ".-.-"),
        ('Ї', ".---."), // Ukrainian
    ];

    /// <summary>Entries whose pattern is unique; every one of them round-trips through encode and decode.</summary>
    internal static readonly (char Char, string Code)[][] Primaries =
    [
        Letters,
        SharedCharacters.Digits,
        SharedCharacters.Punctuation,
    ];

    /// <summary>Encode-only entries that share a pattern with a primary entry.</summary>
    internal static readonly (char Char, string Code)[] Aliases =
    [
        ('Ё', "."),     // same as Е
        ('Ь', "-..-"),  // same as Ъ
        ('Є', "..-.."), // Ukrainian, same as Э
        ('І', ".."),    // Ukrainian, same as И
        ('Ґ', "--."),   // Ukrainian, same as Г
    ];

    internal static MorseAlphabet Build() => MorseAlphabet.Build(Language.Russian, Primaries, Aliases);
}
