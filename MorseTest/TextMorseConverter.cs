using MorseSharp.Converter;

namespace MorseTest
{
    public class TextMorseConverterTest
    {
        [Fact]
        public async void ConvertMorseWithValidString()
        {
            TextMorseConverter converter = new TextMorseConverter(Language.English);
            var morse = await converter.ConvertTextToMorse("Hi There");
            Assert.True(morse.Length > 0);
        }

        [Fact]
        public void ConvertMorseWithNullString()
        {
            TextMorseConverter converter = new TextMorseConverter(Language.English);
            Assert.ThrowsAsync<ArgumentNullException>(async () => await converter.ConvertTextToMorse(null));
        }
        [Fact]
        public void ConvertMorseWithInvalidCharacter()
        {
            var converter = new TextMorseConverter(Language.Kurdish);
            Assert.ThrowsAsync<KeyNotFoundException>(async () => await converter.ConvertTextToMorse("Hi~"));
        }
        [Fact]
        public async void ConvertTextWithValidMorse()
        {
            TextMorseConverter converter = new TextMorseConverter(Language.Kurdish);
            var morse = await converter.ConvertMorseToText("... ..._ ._ .__");
            Assert.True(morse.Length > 0);
        }
        [Fact]
        public void ConvertTextWithNullMorse()
        {
            TextMorseConverter converter = new TextMorseConverter(Language.Kurdish);
            Assert.ThrowsAsync<ArgumentNullException>(async () => await converter.ConvertMorseToText(null));
        }
        [Fact]
        public void ConvertTextWithInvalidMorse()
        {
            TextMorseConverter converter = new TextMorseConverter(Language.Kurdish);
            Assert.ThrowsAsync<ArgumentNullException>(async () => await converter.ConvertMorseToText("........"));
        }



    }
}