using MorseSharp.Converter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseTest
{
    public class MorseTextConverterTest
    {
        [Fact]
        public async void ConvertMorseToValidMorse()
        {
            MorseTextConverter converter = new();
            var sentence = await converter.ConvertMorseToEnglish(".... ..");
            Assert.Equal("HI",sentence);
        }
        [Fact]
        public void ConvertToMorseInValidLetter()
        {
            MorseTextConverter converter = new();
            Assert.ThrowsAsync<Exception>(async () => await converter.ConvertMorseToKurdish(".--.---"));
        }

        [Fact]
        public void ConvertToMorseWithNullArg()
        {
            MorseTextConverter converter = new();
            Assert.ThrowsAsync<ArgumentNullException>(async () => await converter.ConvertMorseToKurdish(null));
        }

    }
}
