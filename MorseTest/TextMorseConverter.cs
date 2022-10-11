using MorseSharp.Converter;

namespace MorseTest
{
    public class TextMorseConverterTest
    {
        //English
        [Fact]
        public async void ConvertMorseWithValidStringEnglish()
        {
            TextMorseConverter converter = new TextMorseConverter();
            var morse = await converter.ConvertToMorseEnglish("Yunis");
            Assert.NotNull(morse);
        }
        [Fact]
        public void ConvertMorseWithNullStringEnglish()
        {
            TextMorseConverter converter = new TextMorseConverter();

            Assert.ThrowsAsync<NullReferenceException>(async () => await converter.ConvertToMorseEnglish(null));
        }
        [Fact]
        public void ConvertMorseWithInvalidCharacterEnglish()
        {
            var converter = new TextMorseConverter();
            Assert.ThrowsAsync<Exception>(async () => await converter.ConvertToMorseEnglish("@"));
        }
        //Kurdish
        [Fact]
        public async void ConvertMorseWithValidStringKurdish()
        {
            TextMorseConverter converter = new TextMorseConverter();
            var morse = await converter.ConvertToMorseKurdish("زانا");
            Assert.NotNull(morse);
        }
        [Fact]
        public void ConvertMorseWithNullStringKurdish()
        {
            TextMorseConverter converter = new TextMorseConverter();

            Assert.ThrowsAsync<NullReferenceException>(async () => await converter.ConvertToMorseEnglish(null));
        }
        [Fact]
        public void ConvertMorseWithInvalidCharacterKurdish()
        {
            var converter = new TextMorseConverter();
            Assert.ThrowsAsync<Exception>(async () => await converter.ConvertToMorseEnglish("@"));
        }
    }
}