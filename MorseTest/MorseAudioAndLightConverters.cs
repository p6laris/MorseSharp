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

        [Fact]
        public void ConvertToAudioSmallerWordSpeed()
        {

            Assert.Throws<SmallerCharSpeedException>(() =>
            {
                Morse.GetConverter()
               .ForLanguage(Language.English)
               .ToMorse("Hello Morse")
               .ToAudio()
               .SetAudioOptions(20, 25, 600)
               .GetBytes(out Span<byte> morse);
            });
        }

        [Fact]
        public void ConvertToAudioWithoutConversion()
        {
            Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToAudio(".... ..")
                .SetAudioOptions (25, 25, 600)
                .GetBytes(out Span<byte> morse);

            Assert.True(morse.Length > 0);
        }
        [Fact]
        public void ConvertToAudioWithoutConversionSmallerCharSpeed()
        {
            Assert.Throws<SmallerCharSpeedException>(() =>
            {
                Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToAudio(".... ..")
                .SetAudioOptions(20, 25, 600)
                .GetBytes(out Span<byte> morse);
            });
           
        }
        [Fact]
        public void ConvertToAudioWithoutConversionNullMorse()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToAudio(null)
                .SetAudioOptions(25, 25, 600)
                .GetBytes(out Span<byte> morse);
            });
        }
        //Light Blinker
        //Latin
        [Fact]
        public void ConvertToLight()
        {
            List<bool> blinkValues = new();
            Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToMorse("Hi")
                .ToLight()
                .SetBlinkerOptions(25, 25)
                .DoBlinks((hasToBlink) =>
                {
                    if (hasToBlink)
                        blinkValues.Add(true);
                    else
                        blinkValues.Add(false);
                });

            Assert.True(blinkValues.Count > 0);
        }
        [Fact]
        public async Task ConvertToLightWithNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToMorse(null)
                .ToLight()
                .SetBlinkerOptions(25, 25)
                .DoBlinks((hasToBlink) =>
                {
                });
            });
            
        }
        [Fact]
        public async Task ConvertToLightWithSmallerCharSpeed()
        {
            await Assert.ThrowsAsync<SmallerCharSpeedException>(async () =>
            {
                await Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToMorse("Hi")
                .ToLight()
                .SetBlinkerOptions(20, 25)
                .DoBlinks((hasToBlink) =>
                {
                });
            });

        }
        [Fact]
        public async Task ConvertToLightWithoutConversionNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToLight(null)
                .SetBlinkerOptions(25, 25)
                .DoBlinks((hasToBlink) =>
                {
                });
            });

        }
        [Fact]
        public async Task ConvertToLightWithoutConversionSmallerCharSpeed()
        {
            await Assert.ThrowsAsync<SmallerCharSpeedException>(async () =>
            {
                await Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToLight(".... ..")
                .SetBlinkerOptions(20, 25)
                .DoBlinks((hasToBlink) =>
                {
                });
            });

        }
    }
}
