using MorseSharp;

namespace ConsoleExample.Demos;

/// <summary>Reading a whole recording back into text.</summary>
internal static class AudioDecodingDemo
{
    public static void Run()
    {
        short[] pcm = AudioSamples.Render16BitMono("Hi", charSpeed: 25, wordSpeed: 15, frequency: 700);

        string text = Morse.GetConverter()
            .ForLanguage(Language.English)
            .FromAudio(pcm, sampleRate: 11025, frequency: 700, wordsPerMinute: 25);
        Console.WriteLine($"Decoded from audio -> {text}");
    }
}
