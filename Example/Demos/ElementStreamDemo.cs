using MorseSharp;

namespace ConsoleExample.Demos;

/// <summary>The timing behind a message, without a light or a WAV file attached to it.</summary>
internal static class ElementStreamDemo
{
    public static void Run()
    {
        Console.Write("Elements for 'OK': ");
        foreach (MorseElement element in Morse.GetConverter()
            .ForLanguage(Language.English).ToMorse("OK").ToLight().SetBlinkerOptions(25, 25).GetElements())
        {
            Console.Write($"{element.Kind}({element.Duration.TotalMilliseconds:F0}ms) ");
        }

        Console.WriteLine();
    }
}
