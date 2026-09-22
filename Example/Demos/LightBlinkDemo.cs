using MorseSharp;

namespace ConsoleExample.Demos;

/// <summary>Real-time light blinking, driven by <c>DoBlinks</c>. Ctrl+C cancels.</summary>
internal static class LightBlinkDemo
{
    public static async Task Run()
    {
        using CancellationTokenSource cts = new();
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

        await Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToLight(".... ..")
            .SetBlinkerOptions(25, 25)
            .DoBlinks(on => Console.BackgroundColor = on ? ConsoleColor.White : ConsoleColor.Black, cts.Token);

        Console.ResetColor();
    }
}
