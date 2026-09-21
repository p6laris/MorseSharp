namespace MorseSharp.Characters;

/*
 * Wabun code, sourced from https://morsedecoder.com/.
 * If you notice an incorrect character please open an issue at https://github.com/p6laris/MorseSharp.
 */
internal static class JapaneseCharacters
{
    private static readonly (char Char, string Code)[] Katakana =
    [
        ('ア', "--.--"), ('カ', ".-.."),  ('サ', "-.-.-"), ('タ', "-."),    ('ナ', ".-."),   ('ハ', "-..."),
        ('マ', "-..-"),  ('ヤ', ".--"),   ('ラ', "..."),   ('ワ', "-.-"),   ('イ', ".-"),    ('キ', "-.-.."),
        ('シ', "--.-."), ('チ', "..-."),  ('ニ', "-.-."),  ('ヒ', "--..-"), ('ミ', "..-.-"), ('リ', "--."),
        ('ヰ', ".-..-"), ('ウ', "..-"),   ('ク', "...-"),  ('ス', "---.-"), ('ツ', ".--."),  ('ヌ', "...."),
        ('フ', "--.."),  ('ム', "-"),     ('ユ', "-..--"), ('ル', "-.--."), ('ン', ".-.-."), ('エ', "-.---"),
        ('ケ', "-.--"),  ('セ', ".---."), ('テ', ".-.--"), ('ネ', "--.-"),  ('ヘ', "."),     ('メ', "-...-"),
        ('レ', "---"),   ('ヱ', ".--.."), ('オ', ".-..."), ('コ', "----"),  ('ソ', "---."),  ('ト', "..-.."),
        ('ノ', "..--"),  ('ホ', "-.."),   ('モ', "-..-."), ('ヨ', "--"),    ('ロ', ".-.-"),  ('ヲ', ".---"),
        ('゛', ".."),    ('゜', "..--."), ('。', ".-.-.."), ('ー', ".--.-"), ('、', ".-.-.-"),
        ('（', "-.--.-"), ('）', ".-..-."),
    ];

    /// <summary>Latin punctuation whose pattern is not taken by a kana.</summary>
    private static readonly (char Char, string Code)[] Punctuation =
    [
        (',', "--..--"), ('?', "..--.."), (';', "-.-.-."), (':', "---..."), ('\'', ".----."),
        ('$', "...-..-"), ('@', ".--.-."), ('¡', "--...-"), ('!', "-.-.--"), ('_', "..--.-"), ('-', "-....-"),
    ];

    /// <summary>Entries whose pattern is unique; every one of them round-trips through encode and decode.</summary>
    internal static readonly (char Char, string Code)[][] Primaries =
    [
        Katakana,
        SharedCharacters.Digits,
        Punctuation,
    ];

    /// <summary>Encode-only entries that share a pattern with a kana.</summary>
    internal static readonly (char Char, string Code)[] Aliases =
    [
        ('.', ".-.-.-"), // same as 、
        ('/', "-..-."),  // same as モ
        ('"', ".-..-."), // same as ）
        ('&', ".-..."),  // same as オ
        ('¿', "..-.-"),  // same as ミ
        ('+', ".-.-."),  // same as ン
        ('=', "-...-"),  // same as メ
    ];

    internal static MorseAlphabet Build() => MorseAlphabet.Build(Language.Japanese, Primaries, Aliases);
}
