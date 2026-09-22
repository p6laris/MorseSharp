using MorseSharp;

namespace ConsoleExample.Demos;

/// <summary>An alphabet defined at runtime, built and used just like a built-in language.</summary>
internal static class CustomAlphabetDemo
{
    public static void Run()
    {
        MorseAlphabet klingon = new MorseAlphabetBuilder("Klingon")
            .Add('a', ".-")
            .Add('b', "-...")
            .AddProsign("KAPLAH", "-.-.-")
            .Build();

        string morse = Morse.GetConverter()
            .ForAlphabet(klingon)
            .ToMorse("ab")
            .Encode();
        Console.WriteLine($"Klingon 'ab' -> {morse}");
    }
}
