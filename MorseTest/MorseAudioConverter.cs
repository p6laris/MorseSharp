using MorseSharp.Audio.Languages;
using MorseSharp.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseTest
{
    public class MorseAudioConverterTest
    {
        //English
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

        //Kurdish
        [Fact]
        public async void ConvertToAudioKurdish()
        {
            MorseSharp.Converter.MorseAudioConverter converter = new(Language.Kurdish);
            var morse = await converter.ConvertMorseToAudio("زانا");
            Assert.True(morse.Length > 0);
        }

        [Fact]
        public void ConvertToAudioWithNullArgumentKurdish()
        {
            MorseSharp.Converter.MorseAudioConverter converter = new(Language.Kurdish);
            Assert.ThrowsAsync<ArgumentNullException>(async () => await converter.ConvertMorseToAudio(null));
        }
    }
}
