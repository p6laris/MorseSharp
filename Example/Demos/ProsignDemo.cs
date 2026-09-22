using MorseSharp;

namespace ConsoleExample.Demos;

/// <summary>Procedural signals, keyed as one unbroken sequence and written with angle brackets.</summary>
internal static class ProsignDemo
{
    public static void Run()
    {
        string morse = Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse("CQ CQ DE W1AW <AR>")
            .Encode();
        Console.WriteLine($"CQ CQ DE W1AW <AR> -> {morse}");
    }
}
