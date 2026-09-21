namespace MorseTest;

/// <summary>
/// Pangram-level checks for each alphabet. Every case also decodes back to the original text, so the forward and
/// reverse directions cannot drift apart.
/// </summary>
public class Languages
{
    /// <param name="expectedDecoded">
    /// What decoding gives back, when that differs from the input because the text uses a letter that shares its
    /// pattern with another one (for example Russian Ё, which is keyed exactly like Е).
    /// </param>
    private static void AssertRoundTrip(Language language, string text, string expectedMorse, string? expectedDecoded = null)
    {
        var conv = Morse.GetConverter().ForLanguage(language);

        string morse = conv.ToMorse(text).Encode();
        Assert.Equal(expectedMorse, morse);
        Assert.Equal(expectedDecoded ?? text.ToUpperInvariant(), conv.Decode(morse));
    }

    [Fact]
    public void EnglishToMorse() => AssertRoundTrip(
        Language.English,
        "THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG",
        "- .... . / --.- ..- .. -.-. -.- / -... .-. --- .-- -. / ..-. --- -..- / .--- ..- -- .--. ... / --- ...- . .-. / - .... . / .-.. .- --.. -.-- / -.. --- --.");

    [Fact]
    public void KurdishToMorse() => AssertRoundTrip(
        Language.Kurdish,
        "کۆژین و ڤیان چوونە بۆ باغەکە ئاوی ساردیان دا بە خرینگ و عەگ و قوڵینگەکان ئینجا پەروازەکانیان فڕاند دواتریش هەموو حاجیلەکانیان چنی",
        "-.-.. .-.- --. .. -. / .-- / ..-.. .. .- -. / ---. .-- .-- -. . / -... .-.- / -... .- ..-- . -.-.. . / ..-..- .- .-- .. / ... .- -.- -.. .. .- -. / -.. .- / -... . / -..- -.- .. -. --.- / .-- / --- . --.- / .-- / ...--- .-- ...- .. -. --.- . -.-.. .- -. / ..-..- .. -. .--- .- / .--. . -.- .-- .- --.. . -.-.. .- -. .. .- -. / ..-. .-. .- -. -.. / -.. .-- .- - -.- .. ---- / -.-. . -- .-- .-- / .... .- .--- .. .-.. . -.-.. .- -. .. .- -. / ---. -. ..");

    [Fact]
    public void KurdishLatinToMorse() => AssertRoundTrip(
        Language.KurdishLatin,
        "Cem vî Fekoyê pîs zêdetir ji çar gulên xweşik hebûn",
        ".--- . -- / ..-.. .. / ..-. . -.-.. .-.- ..-- ..- / .--. .. ... / --.. ..- -.. . - ..-..- -.- / --. ..-..- / ---. .- -.- / --.- .-- .-.. ..- -. / -..- --- . ---- ..-..- -.-.. / -.-. . -... .--.-- -.");

    [Fact]
    public void ArabicToMorse() => AssertRoundTrip(
        Language.Arabic,
        "ابجد هوز حطي كلمن سعفص قرشت ثخذ ضظغ",
        ".- -... .--- -.. / ..-.. .-- ---. / .... ..- .. / -.- .-.. -- -. / ... .-.- ..-. -..- / --.- .-. ---- - / -.-. --- --.. / ...- -.-- --.");

    [Fact]
    public void DeutschToMorse() => AssertRoundTrip(
        Language.Deutsch,
        "VICTOR JAGT ZWÖLF BOXKÄMPFER QUER ÜBER DEN GROẞEN SYLTER DEICH",
        "...- .. -.-. - --- .-. / .--- .- --. - / --.. .-- ---. .-.. ..-. / -... --- -..- -.- .-.- -- .--. ..-. . .-. / --.- ..- . .-. / ..-- -... . .-. / -.. . -. / --. .-. --- ...... . -. / ... -.-- .-.. - . .-. / -.. . .. -.-. ....");

    [Fact]
    public void EspanolToMorse() => AssertRoundTrip(
        Language.Spanish,
        "EL JEFE BUSCÓ EL ÉXTASIS EN UN IMPREVISTO BAÑO DE WHISKY Y GOZÓ COMO UN DUQUE",
        ". .-.. / .--- . ..-. . / -... ..- ... -.-. ---. / . .-.. / ..-.. -..- - .- ... .. ... / . -. / ..- -. / .. -- .--. .-. . ...- .. ... - --- / -... .- --.-- --- / -.. . / .-- .... .. ... -.- -.-- / -.-- / --. --- --.. ---. / -.-. --- -- --- / ..- -. / -.. ..- --.- ..- .");

    [Fact]
    public void FrancaisToMorse() => AssertRoundTrip(
        Language.French,
        "PORTEZ CE VIEUX WHISKY AU JUGE BLOND QUI FUME",
        ".--. --- .-. - . --.. / -.-. . / ...- .. . ..- -..- / .-- .... .. ... -.- -.-- / .- ..- / .--- ..- --. . / -... .-.. --- -. -.. / --.- ..- .. / ..-. ..- -- .");

    [Fact]
    public void ItalianoToMorse() => AssertRoundTrip(
        Language.Italian,
        "PRANZO D'ACQUA FA VOLTI SGHEMBI",
        ".--. .-. .- -. --.. --- / -.. .----. .- -.-. --.- ..- .- / ..-. .- / ...- --- .-.. - .. / ... --. .... . -- -... ..");

    [Fact]
    public void JapaneseToMorse() => AssertRoundTrip(
        Language.Japanese,
        "アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲン",
        "--.-- .- ..- -.--- .-... .-.. -.-.. ...- -.-- ---- -.-.- --.-. ---.- .---. ---. -. ..-. .--. .-.-- ..-.. .-. -.-. .... --.- ..-- -... --..- --.. . -.. -..- ..-.- - -...- -..-. .-- -..-- -- ... --. -.--. --- .-.- -.- .--- .-.-.");

    [Fact]
    public void PortuguesToMorse() => AssertRoundTrip(
        Language.Portugues,
        "UM PEQUENO JABUTI XERETA VIU DEZ CEGONHAS FELIZES",
        "..- -- / .--. . --.- ..- . -. --- / .--- .- -... ..- - .. / -..- . .-. . - .- / ...- .. ..- / -.. . --.. / -.-. . --. --- -. .... .- ... / ..-. . .-.. .. --.. . ...");

    [Fact]
    public void RussianToMorse() => AssertRoundTrip(
        Language.Russian,
        "СЪЕШЬ ЕЩЁ ЭТИХ МЯГКИХ ФРАНЦУЗСКИХ БУЛОК, ДА ВЫПЕЙ ЖЕ ЧАЮ.",
        "... -..- . ---- -..- / . --.- . / ..-.. - .. .... / -- .-.- --. -.- .. .... / ..-. .-. .- -. -.-. ..- --.. ... -.- .. .... / -... ..- .-.. --- -.- --..-- / -.. .- / .-- -.-- .--. . .--- / ...- . / ---. .- ..-- .-.-.-",
        // Ь is keyed like Ъ and Ё like Е, so both come back as the letter that owns the pattern.
        "СЪЕШЪ ЕЩЕ ЭТИХ МЯГКИХ ФРАНЦУЗСКИХ БУЛОК, ДА ВЫПЕЙ ЖЕ ЧАЮ.");
}
