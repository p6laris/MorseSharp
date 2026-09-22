using MorseSharp;

namespace ConsoleExample.Demos;

/// <summary>Pushing samples in as they arrive, and reading characters out as soon as they land.</summary>
internal static class StreamingDecodingDemo
{
    public static void Run()
    {
        StreamingMorseDecoder decoder = Morse.GetConverter()
            .ForLanguage(Language.English)
            .CreateAudioDecoder(sampleRate: 11025, frequency: 700, wordsPerMinute: 25);

        short[] samples = AudioSamples.Render16BitMono("SOS", charSpeed: 25, wordSpeed: 25, frequency: 700);

        Console.Write("Streaming decode -> ");
        foreach (short[] chunk in samples.Chunk(256))
        {
            decoder.Write(chunk);
            while (decoder.TryRead(out char character))
                Console.Write(character);
        }

        decoder.Flush();
        while (decoder.TryRead(out char tail))
            Console.Write(tail);

        Console.WriteLine();
    }
}
