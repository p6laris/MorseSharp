using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using MorseSharp;

namespace ConsoleExample
{
    [ShortRunJob]
    [MemoryDiagnoser]
    public class MorseBenchmark
    {
        

        [Benchmark]
        public void ToMorse()
        {
            Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToMorse("Hi")
                .Encode();
        }

        [Benchmark]
        public void ToText()
        {
            Morse.GetConverter()
                .ForLanguage(Language.English)
                .Decode(".... ..");
        }

        [Benchmark]
        public void ToAudio()
        {
            Morse.GetConverter()
               .ForLanguage(Language.English)
               .ToAudio(".... ..")
               .SetAudioOptions(25, 25, 600)
               .GetBytes(out Span<byte> morse);
        }
    }
}
