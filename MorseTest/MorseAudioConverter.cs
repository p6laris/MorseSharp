namespace MorseTest
{
    public class MorseAudioConverterTest
    {
        //Latin
        [Fact]
        public void ConvertToAudioEnglish()
        {
            Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToMorse("Hello Morse")
                .ToAudio()
                .SetAudioOptions(25, 25, 600)
                .GetBytes(out Span<byte> morse);

            Assert.True(morse.Length > 0);
        }

        //NonLatin
        [Fact]
        public void ConvertToAudioKurdish()
        {
            Morse.GetConverter()
                 .ForLanguage(Language.Kurdish)
                 .ToMorse("کووی")
                 .ToAudio()
                 .SetAudioOptions(25, 25, 600)
                 .GetBytes(out Span<byte> morse);

            Assert.True(morse.Length > 0);
        }
    }
}
