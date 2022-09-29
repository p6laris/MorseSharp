using MorseSharp.MorseConverter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseTest
{
    public class MorseAudioConverterTest
    {
        [Fact]
        public async void ConvertToAudio()
        {
            MorseSharp.MorseConverter.MorseAudioConverter converter = new();
            var morse = await converter.ConvertMorseToAudio("Hello Morse");
            Assert.True(morse.Length > 0);
        }

        [Fact]
        public void ConvertToAudioWithNullArgument()
        {
            MorseSharp.MorseConverter.MorseAudioConverter converter = new();
            Assert.ThrowsAsync<ArgumentNullException>(async () => await converter.ConvertMorseToAudio(null));
        }

    }
}
