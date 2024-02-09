namespace MorseTest
{
    public class Languages
    {
        [Fact]
        public void EnglishToMorse()
        {

            var morse = Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToMorse("The quick brown fox jumps over the lazy dog")
                .Encode();

            Assert.Equal("- .... . / --.- ..- .. -.-. -.- / -... .-. --- .-- -. / ..-. --- -..- / .--- ..- -- .--. ... / --- ...- . .-. / - .... . / .-.. .- --.. -.-- / -.. --- --.",
                morse);
        }
        [Fact]
        public void KurdishToMorse()
        {

            var morse = Morse.GetConverter()
                .ForLanguage(Language.Kurdish)
                .ToMorse("کۆژین و ڤیان چوونە بۆ باغەکە ئاوی ساردیان دا بە خرینگ و عەگ و قوڵینگەکان ئینجا پەروازەکانیان فڕاند دواتریش هەموو حاجیلەکانیان چنی")
                .Encode();

            Assert.Equal("-.... .-.- --. .. -. / .-- / ..-.. .. .- -. / ---. .-- .-- -. . / -... .-.- / -... .- ..-- . -.... . / ..-.. .- .-- .. / ... .- -.- -.. .. .- -. / -.. .- / -... . / -..- -.- .. -. --.- / .-- / --- . --.- / .-- / ...--- .-- ...- .. -. --.- . -.... .- -. / ..-.. .. -. .--- .- / .--. . -.- .-- .- --.. . -.... .- -. .. .- -. / ..-. .-. .- -. -.. / -.. .-- .- - -.- .. ---- / -.-. . -- .-- .-- / .... .- .--- .. .-.. . -.... .- -. .. .- -. / ---. -. ..",
                morse);
        }
        [Fact]
        public void KurdishLatinToMorse()
        {
            var morse = Morse.GetConverter()
                 .ForLanguage(Language.KurdishLatin)
                 .ToMorse("Cem vî Fekoyê pîs zêdetir ji çar gulên xweşik hebûn")
                 .Encode();

            Assert.Equal(".--- . -- / ..-.. .. / ..-. . -.... .-.- ..-- ..- / .--. .. ... / --.. ..- -.. . - ..-.. -.- / --. ..-.. / ---. .- -.- / --.- .-- .-.. ..- -. / -..- --- . ---- ..-.. -.... / -.-. . -... .--.-- -.",
                morse);
        }
        [Fact]
        public void ArabicToMorse()
        {
            var morse = Morse.GetConverter()
                 .ForLanguage(Language.Arabic)
                 .ToMorse("ابجد هوز حطي كلمن سعفص قرشت ثخذ ضظغ")
                 .Encode();

            Assert.Equal(".- -... .--- -.. / ..-.. .-- ---. / .... ..- .. / -.- .-.. -- -. / ... .-.- ..-. -..- / --.- .-. ---- - / -.-. --- --.. / ...- -.-- --.",
                morse);
        }
        [Fact]
        public void DeutschToMorse()
        {
            var morse = Morse.GetConverter()
                 .ForLanguage(Language.Deutsch)
                 .ToMorse("Victor jagt zwölf Boxkämpfer quer über den groẞen Sylter Deich")
                 .Encode();

            Assert.Equal("...- .. -.-. - --- -.- / .--- .- --. - / --.. .-- ---. .-.. ..-. / -... --- -..- -.- .-.- -- .--. ..-. . -.- / --.- ..- . -.- / ..-- -... . -.- / -.. . -. / --. -.- --- ...... . -. / ... -.-- .-.. - . -.- / -.. . .. -.-. ....",
                morse);
        }

        [Fact]
        public void EspanolToMorse()
        {
            var morse = Morse.GetConverter()
                 .ForLanguage(Language.Espanol)
                 .ToMorse("El jefe buscó el éxtasis en un imprevisto baño de whisky y gozó como un duque")
                 .Encode();

            Assert.Equal(". .-.. / .--- . ..-. . / -... ..- ... -.-. ---. / . .-.. / ..-.. -..- - .- ... .. ... / . -. / ..- -. / .. -- .--. .-. . ...- .. ... - --- / -... .- --.-- --- / -.. . / .-- .... .. ... -.- -.-- / -.-- / --. --- --.. ---. / -.-. --- -- --- / ..- -. / -.. ..- --.- ..- .",
                morse);
        }
        [Fact]
        public void FrancaisToMorse()
        {
            var morse = Morse.GetConverter()
                .ForLanguage(Language.Francais)
                .ToMorse("Portez ce vieux whisky au juge blond qui fume")
                .Encode();

            Assert.Equal(".--. --- .-. - . --.. / -.-. . / ...- .. . ..- -..- / .-- .... .. ... -.- -.-- / .- ..- / .--- ..- --. . / -... .-.. --- -. -.. / --.- ..- .. / ..-. ..- -- .",
                morse);
        }
        [Fact]
        public void ItalianoToMorse()
        {
            var morse = Morse.GetConverter()
                .ForLanguage(Language.Italiano)
                .ToMorse("Pranzo d'acqua fa volti sghembi")
                .Encode();

            Assert.Equal(".--. .-. .- -. --.. --- / -.. .----. .- -.-. --.- ..- .- / ..-. .- / ...- --- .-.. - .. / ... --. .... . -- -... ..",
                morse);
        }
        [Fact]
        public void JapaneseToMorse()
        {
            var morse = Morse.GetConverter()
                .ForLanguage(Language.Japanese)
                .ToMorse("アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲン")
                .Encode();

            Assert.Equal("--.-- .- ..- -.--- .-... .-.. -.-.. ...- -.-- ---- -.-.- --.-. ---.- .---. ---. -. ..-. .--. .-.-- ..-.. .-. -.-. .... --.- ..-- -... --..- --.. . -.. -..- ..-.- - -...- -..-. .-- -..-- -- ... --. -.--. --- .-.- -.- .--- .-.-.",
                morse);
        }

        [Fact]
        public void PortuguesToMorse()
        {
            var morse = Morse.GetConverter()
                 .ForLanguage(Language.Portugues)
                 .ToMorse("Um pequeno jabuti xereta viu dez cegonhas felizes")
                 .Encode();

            Assert.Equal("..- -- / .--. . --.- ..- . -. --- / .--- .- -... ..- - .. / -..- . .-. . - .- / ...- .. ..- / -.. . --.. / -.-. . --. --- -. .... .- ... / ..-. . .-.. .. --.. . ...",
                morse);
        }
        [Fact]
        public async void RussianToMorse()
        {

            var morse = Morse.GetConverter()
                .ForLanguage(Language.Russian)
                .ToMorse("Съешь ещё этих мягких французских булок, да выпей же чаю.")
                .Encode(); 

            Assert.Equal("... -..- . ---- -..- / . --.- . / ..-.. - .. .... / -- .-.- --. -.- .. .... / ..-. .-. .- -. -.-. ..- --.. ... -.- .. .... / -... ..- .-.. --- -.- .-.-.- / -.. .- / .-- -.-- .--. . .--- / ...- . / ---. .- ..-- ......", morse);
        }
    }
}
