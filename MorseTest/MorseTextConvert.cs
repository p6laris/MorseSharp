using MorseSharp.Converter;

namespace MorseTest
{
    public class MorseTextConvertTest
    {
        //English
        [Fact]
        public async void ConvertMorseWithValidStringEnglish()
        {
            MorseTextConverter converter = new MorseTextConverter();
            var morse = await converter.ConvertToMorseEnglish("Yunis");
            Assert.NotNull(morse);
        }
        [Fact]
        public void ConvertMorseWithNullStringEnglish()
        {
            MorseTextConverter converter = new MorseTextConverter();

            Assert.ThrowsAsync<NullReferenceException>(async () => await converter.ConvertToMorseEnglish(null));
        }
        [Fact]
        public void ConvertMorseWithInvalidCharacterEnglish()
        {
            var converter = new MorseTextConverter();
            Assert.ThrowsAsync<Exception>(async () => await converter.ConvertToMorseEnglish("@"));
        }
        //Kurdish
        [Fact]
        public async void ConvertMorseWithValidStringKurdish()
        {
            MorseTextConverter converter = new MorseTextConverter();
            var morse = await converter.ConvertToMorseKurdish("زانا");
            Assert.NotNull(morse);
        }
        [Fact]
        public void ConvertMorseWithNullStringKurdish()
        {
            MorseTextConverter converter = new MorseTextConverter();

            Assert.ThrowsAsync<NullReferenceException>(async () => await converter.ConvertToMorseEnglish(null));
        }
        [Fact]
        public void ConvertMorseWithInvalidCharacterKurdish()
        {
            var converter = new MorseTextConverter();
            Assert.ThrowsAsync<Exception>(async () => await converter.ConvertToMorseEnglish("@"));
        }
    }
}