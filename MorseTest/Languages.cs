using MorseSharp.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseTest
{
    public class Languages
    {
        [Fact]
        public async void EnglishToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.English);

            var morse = await Converter.ConvertTextToMorse("Hi");

            Assert.Equal(".... .. ", morse);
        }
        [Fact]
        public async void KurdishToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Kurdish);

            var morse = await Converter.ConvertTextToMorse("سڵاو");

            Assert.Equal("... ..._ ._ .__ ", morse);
        }
        [Fact]
        public async void ArabicToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Arabic);

            var morse = await Converter.ConvertTextToMorse("مرحبا");

            Assert.Equal("__ ._. .... _... ._ ", morse);
        }
        [Fact]
        public async void DeutschToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Deutsch);

            var morse = await Converter.ConvertTextToMorse("hallo");

            Assert.Equal(".... ._ ._.. ._.. ___ ", morse);
        }

        [Fact]
        public async void EspanolToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Espanol);

            var morse = await Converter.ConvertTextToMorse("hola");

            Assert.Equal(".... ___ ._.. ._ ", morse);
        }
        [Fact]
        public async void FrancaisToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Espanol);

            var morse = await Converter.ConvertTextToMorse("salut");

            Assert.Equal("... ._ ._.. .._ _ ", morse);
        }
        [Fact]
        public async void ItalianoToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Italiano);

            var morse = await Converter.ConvertTextToMorse("ciao");

            Assert.Equal("_._. .. ._ ___ ", morse);
        }
        [Fact]
        public async void JapaneseToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Japanese);

            var morse = await Converter.ConvertTextToMorse("ヘロ");

            Assert.Equal(". ._._ ", morse);
        }

        [Fact]
        public async void PortuguesToMorse()
        {
            TextMorseConverter Converter = new TextMorseConverter(Language.Portugues);

            var morse = await Converter.ConvertTextToMorse("olá");

            Assert.Equal("___ ._.. .__._ ", morse);
        }
    }
}
