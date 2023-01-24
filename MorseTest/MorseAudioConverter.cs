using MorseSharp.Converter;

namespace MorseTest
{
    public class MorseAudioConverterTest
    {
        //Latin
        [Fact]
        public async void ConvertToAudioEnglish()
        {
            MorseSharp.Converter.MorseAudioConverter converter = new();
            var morse = await converter.ConvertMorseToAudio("Hello Morse");
            Assert.True(morse.Length > 0);
        }

        [Fact]
        public void ConvertToAudioWithNullArgumentEnglish()
        {
            MorseSharp.Converter.MorseAudioConverter converter = new();
            Assert.ThrowsAsync<ArgumentNullException>(async () => await converter.ConvertMorseToAudio(null));
        }

        //NonLatin
        [Fact]
        public async void ConvertToAudioKurdish()
        {
            MorseAudioConverter converter = new(Language.Kurdish);
            var morse = await converter.ConvertMorseToAudio("زانا");
            Assert.True(morse.Length > 0);
        }

        //Argumnet Null Exception
        [Fact]
        public void ConvertToAudioWithNullArgumentKurdish()
        {
            MorseAudioConverter converter = new(Language.Kurdish);
            Assert.ThrowsAsync<ArgumentNullException>(async () => await converter.ConvertMorseToAudio(null));
        }
    }
}
