using MorseSharp.MorseConverter;

namespace MorseTest
{
    public class MorseConvert
    {
        [Fact]
        public async void ConvertMorseWithValidString()
        {
            MorseTextConverter converter = new MorseTextConverter();
            var morse = await converter.ConvertToMorse("Yunis");
            Assert.NotNull(morse);
        }
        [Fact]
        public void ConvertMorseWithNullString()
        {
            MorseTextConverter converter = new MorseTextConverter();

            Assert.ThrowsAsync<NullReferenceException>(async () => await converter.ConvertToMorse(null));
        }
        [Fact]
        public void ConvertMorseWithInvalidCharacter()
        {
            var converter = new MorseTextConverter();
            Assert.ThrowsAsync<Exception>(async () => await converter.ConvertToMorse("@"));
        }
    }
}