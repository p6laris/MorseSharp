using MorseSharp.MorseConverter;

namespace MorseTest
{
    public class MorseTextConvertTest
    {
        [Fact]
        public async void ConvertMorseWithValidString()
        {
            MorseTextConverter converter = new MorseTextConverter();
            var morse = await converter.ConvertToMorseEnglish("Yunis");
            Assert.NotNull(morse);
        }
        [Fact]
        public void ConvertMorseWithNullString()
        {
            MorseTextConverter converter = new MorseTextConverter();

            Assert.ThrowsAsync<NullReferenceException>(async () => await converter.ConvertToMorseEnglish(null));
        }
        [Fact]
        public void ConvertMorseWithInvalidCharacter()
        {
            var converter = new MorseTextConverter();
            Assert.ThrowsAsync<Exception>(async () => await converter.ConvertToMorseEnglish("@"));
        }
    }
}