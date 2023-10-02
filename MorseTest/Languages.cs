using MorseSharp.Converter;

namespace MorseTest
{
    public class Languages
    {
        [Fact]
        public async void EnglishToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.English);

            var morse = await Converter.ConvertTextToMorse("The quick brown fox jumps over the lazy dog");

            Assert.Equal("_ .... . / __._ .._ .. _._. _._ / _... ._. ___ .__ _. / .._. ___ _.._ / .___ .._ __ .__. ... / ___ ..._ . ._. / _ .... . / ._.. ._ __.. _.__ / _.. ___ __. ",
                morse);
        }
        [Fact]
        public async void KurdishToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Kurdish);

            var morse = await Converter.ConvertTextToMorse("کۆژین و ڤیان چوونە بۆ باغەکە ئاوی ساردیان دا بە خرینگ و عەگ و قوڵینگەکان ئینجا پەروازەکانیان فڕاند دواتریش هەموو حاجیلەکانیان چنی");

            Assert.Equal("_.... ._._ __. .. _. / .__ / .._.. .. ._ _. / ___. .__ .__ _. . / _... ._._ / _... ._ ..__ . _.... . / .._.. ._ .__ .. / ... ._ _._ _.. .. ._ _. / _.. ._ / _... . / _.._ _._ .. _. __._ / .__ / ___ . __._ / .__ / ...___ .__ ..._ .. _. __._ . _.... ._ _. / .._.. .. _. .___ ._ / .__. . _._ .__ ._ __.. . _.... ._ _. .. ._ _. / .._. ._. ._ _. _.. / _.. .__ ._ _ _._ .. ____ / _._. . __ .__ .__ / .... ._ .___ .. ._.. . _.... ._ _. .. ._ _. / ___. _. .. ",
                morse);
        }
        [Fact]
        public async void KurdishLatinToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.KurdishLatin);

            var morse = await Converter.ConvertTextToMorse("Cem vî Fekoyê pîs zêdetir ji çar gulên xweşik hebûn");

            Assert.Equal(".___ . __ / .._.. .. / .._. . _.... ._._ ..__ .._ / .__. .. ... / __.. .._ _.. . _ .._.. _._ / __. .._.. / ___. ._ _._ / __._ .__ ._.. .._ _. / _.._ ___ . ____ .._.. _.... / _._. . _... .__.__ _. ",
                morse);
        }
        [Fact]
        public async void ArabicToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Arabic);

            var morse = await Converter.ConvertTextToMorse("ابجد هوز حطي كلمن سعفص قرشت ثخذ ضظغ");

            Assert.Equal("._ _... .___ _.. / .._.. .__ ___. / .... .._ .. / _._ ._.. __ _. / ... ._._ .._. _.._ / __._ ._. ____ _ / _._. ___ __.. / ..._ _.__ __. ",
                morse);
        }
        [Fact]
        public async void DeutschToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Deutsch);

            var morse = await Converter.ConvertTextToMorse("Victor jagt zwölf Boxkämpfer quer über den groẞen Sylter Deich");

            Assert.Equal("..._ .. _._. _ ___ _._ / .___ ._ __. _ / __.. .__ ___. ._.. .._. / _... ___ _.._ _._ ._._ __ .__. .._. . _._ / __._ .._ . _._ / ..__ _... . _._ / _.. . _. / __. _._ ___ ...... . _. / ... _.__ ._.. _ . _._ / _.. . .. _._. .... ",
                morse);
        }

        [Fact]
        public async void EspanolToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Espanol);

            var morse = await Converter.ConvertTextToMorse("El jefe buscó el éxtasis en un imprevisto baño de whisky y gozó como un duque");

            Assert.Equal(". ._.. / .___ . .._. . / _... .._ ... _._. ___. / . ._.. / .._.. _.._ _ ._ ... .. ... / . _. / .._ _. / .. __ .__. ._. . ..._ .. ... _ ___ / _... ._ __.__ ___ / _.. . / .__ .... .. ... _._ _.__ / _.__ / __. ___ __.. ___. / _._. ___ __ ___ / .._ _. / _.. .._ __._ .._ . ",
                morse);
        }
        [Fact]
        public async void FrancaisToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Francais);

            var morse = await Converter.ConvertTextToMorse("Portez ce vieux whisky au juge blond qui fume");

            Assert.Equal(".__. ___ ._. _ . __.. / _._. . / ..._ .. . .._ _.._ / .__ .... .. ... _._ _.__ / ._ .._ / .___ .._ __. . / _... ._.. ___ _. _.. / __._ .._ .. / .._. .._ __ . ",
                morse);
        }
        [Fact]
        public async void ItalianoToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Italiano);

            var morse = await Converter.ConvertTextToMorse("Pranzo d'acqua fa volti sghembi");

            Assert.Equal(".__. ._. ._ _. __.. ___ / _.. .____. ._ _._. __._ .._ ._ / .._. ._ / ..._ ___ ._.. _ .. / ... __. .... . __ _... .. ",
                morse);
        }
        [Fact]
        public async void JapaneseToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Japanese);

            var morse = await Converter.ConvertTextToMorse("アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲン");

            Assert.Equal("__.__ ._ .._ _.___ ._... ._.. _._.. ..._ _.__ ____ _._._ __._. ___._ .___. ___. _. .._. .__. ._.__ .._.. ._. _._. .... __._ ..__ _... __.._ __.. . _.. _.._ .._._ _ _..._ _.._. .__ _..__ __ ... __. _.__. ___ ._._ _._ .___ ._._. ",
                morse);
        }

        [Fact]
        public async void PortuguesToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Portugues);

            var morse = await Converter.ConvertTextToMorse("Um pequeno jabuti xereta viu dez cegonhas felizes");

            Assert.Equal(".._ __ / .__. . __._ .._ . _. ___ / .___ ._ _... .._ _ .. / _.._ . ._. . _ ._ / ..._ .. .._ / _.. . __.. / _._. . __. ___ _. .... ._ ... / .._. . ._.. .. __.. . ... ",
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
