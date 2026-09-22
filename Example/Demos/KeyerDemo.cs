using MorseSharp;

namespace ConsoleExample.Demos;

/// <summary>Paddle presses in, elements out, and a decoder that turns them back into text.</summary>
internal static class KeyerDemo
{
    public static void Run()
    {
        IambicKeyer keyer = new(wordsPerMinute: 25, KeyerMode.B);
        KeyerDecoder decoder = new(Language.English, wordsPerMinute: 25);

        // Key "N" (-.) by hand: dash, then dot, each followed by the gap that separates them.
        keyer.Paddles(dot: false, dash: true);
        keyer.TryRead(out MorseElement dash);
        decoder.Add(dash);
        keyer.Paddles(dot: false, dash: false);
        keyer.TryRead(out MorseElement gap);
        decoder.Add(gap);

        keyer.Paddles(dot: true, dash: false);
        keyer.TryRead(out MorseElement dot);
        decoder.Add(dot);
        keyer.Paddles(dot: false, dash: false);

        decoder.Flush(out char character, out string? prosign);
        Console.WriteLine($"Keyed by hand -> {prosign ?? character.ToString()}");
    }
}
