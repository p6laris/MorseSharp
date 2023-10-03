using MorseSharp.Converter;

namespace MorseTest
{
    public class TextMorseConverterTest
    {
        [Fact]
        public void ConvertMorseWithValidString()
        {
            TextMorseConverter converter = new TextMorseConverter(Language.English);
            var morse =  converter.ConvertTextToMorse("Hi There");
            Assert.True(morse.Length > 0);
        }

        [Fact]
        public void ConvertMorseWithNullString()
        {
            TextMorseConverter converter = new TextMorseConverter(Language.English);
            Assert.Throws<ArgumentNullException>(() => converter.ConvertTextToMorse(null));
        }
        [Fact]
        public void ConvertMorseWithInvalidCharacter()
        {
            var converter = new TextMorseConverter(Language.Kurdish);
            Assert.Throws<KeyNotFoundException>(() => converter.ConvertTextToMorse("Hi~"));
        }
        [Fact]
        public void ConvertTextWithValidMorse()
        {
            TextMorseConverter converter = new TextMorseConverter(Language.Kurdish);
            var morse = converter.ConvertMorseToText("... ..._ ._ .__");
            Assert.True(morse.Length > 0);
        }
        [Fact]
        public void ConvertTextWithNullMorse()
        {
            TextMorseConverter converter = new TextMorseConverter(Language.Kurdish);
            Assert.Throws<ArgumentNullException>(() => converter.ConvertMorseToText(null));
        }
        [Fact]
        public void ConvertTextWithInvalidMorse()
        {
            TextMorseConverter converter = new TextMorseConverter(Language.Kurdish);
            Assert.Throws<KeyNotFoundException>(() => converter.ConvertMorseToText("........"));
        }



    }
}