using MorseSharp;

namespace ConsoleExample.Demos;

/// <summary>Text to Morse, and Morse back to text.</summary>
internal static class EncodingDemo
{
    public static void Run()
    {
        string morse = Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse("Hi")
            .Encode();
        Console.WriteLine($"Hi -> {morse}");

        string text = Morse.GetConverter()
            .ForLanguage(Language.English)
            .Decode(".... ..");
        Console.WriteLine($".... .. -> {text}");
    }
}
