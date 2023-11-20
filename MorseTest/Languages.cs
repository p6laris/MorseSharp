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

            Assert.Equal("_ .... . / __._ .._ .. _._. _._ / _... ._. ___ .__ _. / .._. ___ _.._ / .___ .._ __ .__. ... / ___ ..._ . ._. / _ .... . / ._.. ._ __.. _.__ / _.. ___ __.",
                morse);
        }
        [Fact]
        public void KurdishToMorse()
        {

            var morse = Morse.GetConverter()
                .ForLanguage(Language.Kurdish)
                .ToMorse("کۆژین و ڤیان چوونە بۆ باغەکە ئاوی ساردیان دا بە خرینگ و عەگ و قوڵینگەکان ئینجا پەروازەکانیان فڕاند دواتریش هەموو حاجیلەکانیان چنی")
                .Encode();

            Assert.Equal("_.... ._._ __. .. _. / .__ / .._.. .. ._ _. / ___. .__ .__ _. . / _... ._._ / _... ._ ..__ . _.... . / .._.. ._ .__ .. / ... ._ _._ _.. .. ._ _. / _.. ._ / _... . / _.._ _._ .. _. __._ / .__ / ___ . __._ / .__ / ...___ .__ ..._ .. _. __._ . _.... ._ _. / .._.. .. _. .___ ._ / .__. . _._ .__ ._ __.. . _.... ._ _. .. ._ _. / .._. ._. ._ _. _.. / _.. .__ ._ _ _._ .. ____ / _._. . __ .__ .__ / .... ._ .___ .. ._.. . _.... ._ _. .. ._ _. / ___. _. ..",
                morse);
        }
        [Fact]
        public void KurdishLatinToMorse()
        {
            var morse = Morse.GetConverter()
                 .ForLanguage(Language.KurdishLatin)
                 .ToMorse("Cem vî Fekoyê pîs zêdetir ji çar gulên xweşik hebûn")
                 .Encode();

            Assert.Equal(".___ . __ / .._.. .. / .._. . _.... ._._ ..__ .._ / .__. .. ... / __.. .._ _.. . _ .._.. _._ / __. .._.. / ___. ._ _._ / __._ .__ ._.. .._ _. / _.._ ___ . ____ .._.. _.... / _._. . _... .__.__ _.",
                morse);
        }
        [Fact]
        public void ArabicToMorse()
        {
            var morse = Morse.GetConverter()
                 .ForLanguage(Language.Arabic)
                 .ToMorse("ابجد هوز حطي كلمن سعفص قرشت ثخذ ضظغ")
                 .Encode();

            Assert.Equal("._ _... .___ _.. / .._.. .__ ___. / .... .._ .. / _._ ._.. __ _. / ... ._._ .._. _.._ / __._ ._. ____ _ / _._. ___ __.. / ..._ _.__ __.",
                morse);
        }
        [Fact]
        public void DeutschToMorse()
        {
            var morse = Morse.GetConverter()
                 .ForLanguage(Language.Deutsch)
                 .ToMorse("Victor jagt zwölf Boxkämpfer quer über den groẞen Sylter Deich")
                 .Encode();

            Assert.Equal("..._ .. _._. _ ___ _._ / .___ ._ __. _ / __.. .__ ___. ._.. .._. / _... ___ _.._ _._ ._._ __ .__. .._. . _._ / __._ .._ . _._ / ..__ _... . _._ / _.. . _. / __. _._ ___ ...... . _. / ... _.__ ._.. _ . _._ / _.. . .. _._. ....",
                morse);
        }

        [Fact]
        public void EspanolToMorse()
        {
            var morse = Morse.GetConverter()
                 .ForLanguage(Language.Espanol)
                 .ToMorse("El jefe buscó el éxtasis en un imprevisto baño de whisky y gozó como un duque")
                 .Encode();

            Assert.Equal(". ._.. / .___ . .._. . / _... .._ ... _._. ___. / . ._.. / .._.. _.._ _ ._ ... .. ... / . _. / .._ _. / .. __ .__. ._. . ..._ .. ... _ ___ / _... ._ __.__ ___ / _.. . / .__ .... .. ... _._ _.__ / _.__ / __. ___ __.. ___. / _._. ___ __ ___ / .._ _. / _.. .._ __._ .._ .",
                morse);
        }
        [Fact]
        public void FrancaisToMorse()
        {
            var morse = Morse.GetConverter()
                .ForLanguage(Language.Francais)
                .ToMorse("Portez ce vieux whisky au juge blond qui fume")
                .Encode();

            Assert.Equal(".__. ___ ._. _ . __.. / _._. . / ..._ .. . .._ _.._ / .__ .... .. ... _._ _.__ / ._ .._ / .___ .._ __. . / _... ._.. ___ _. _.. / __._ .._ .. / .._. .._ __ .",
                morse);
        }
        [Fact]
        public void ItalianoToMorse()
        {
            var morse = Morse.GetConverter()
                .ForLanguage(Language.Italiano)
                .ToMorse("Pranzo d'acqua fa volti sghembi")
                .Encode();

            Assert.Equal(".__. ._. ._ _. __.. ___ / _.. .____. ._ _._. __._ .._ ._ / .._. ._ / ..._ ___ ._.. _ .. / ... __. .... . __ _... ..",
                morse);
        }
        [Fact]
        public void JapaneseToMorse()
        {
            var morse = Morse.GetConverter()
                .ForLanguage(Language.Japanese)
                .ToMorse("アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲン")
                .Encode();

            Assert.Equal("__.__ ._ .._ _.___ ._... ._.. _._.. ..._ _.__ ____ _._._ __._. ___._ .___. ___. _. .._. .__. ._.__ .._.. ._. _._. .... __._ ..__ _... __.._ __.. . _.. _.._ .._._ _ _..._ _.._. .__ _..__ __ ... __. _.__. ___ ._._ _._ .___ ._._.",
                morse);
        }

        [Fact]
        public void PortuguesToMorse()
        {
            var morse = Morse.GetConverter()
                 .ForLanguage(Language.Portugues)
                 .ToMorse("Um pequeno jabuti xereta viu dez cegonhas felizes")
                 .Encode();

            Assert.Equal(".._ __ / .__. . __._ .._ . _. ___ / .___ ._ _... .._ _ .. / _.._ . ._. . _ ._ / ..._ .. .._ / _.. . __.. / _._. . __. ___ _. .... ._ ... / .._. . ._.. .. __.. . ...",
                morse);
        }
        [Fact]
        public async void RussianToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Russian);

            var morse = await Converter.ConvertTextToMorse("Съешь ещё этих мягких французских булок, да выпей же чаю.");

            Assert.Equal("... _.._ . ____ _.._ / . __._ . / .._.. _ .. .... / __ ._._ __. _._ .. .... / .._. ._. ._ _. _._. .._ __.. ... _._ .. .... / _... .._ ._.. ___ _._ ._._._ / _.. ._ / .__ _.__ .__. . .___ / ..._ . / ___. ._ ..__ ...... ", morse);
        }
    }
}
