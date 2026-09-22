using MorseSharp;
using MorseSharp.Audio;

namespace ConsoleExample.Demos;

/// <summary>A configurable output format, with a fade that removes the key click.</summary>
internal static class AudioFormatDemo
{
    public static void Run()
    {
        AudioFormat format = new(SampleRate: 44100, BitDepth: AudioBitDepth.Float32, EdgeMilliseconds: 5);
        byte[] wav = Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse("Hi")
            .ToAudio()
            .SetAudioOptions(charSpeed: 25, wordSpeed: 15, frequency: 700, format: format)
            .GetBytes();

        using (FileStream file = File.Create("hi.wav"))
            file.Write(wav);

        Console.WriteLine($"Wrote hi.wav ({wav.Length} bytes, 44.1 kHz float32)");
    }
}
