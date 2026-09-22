using MorseSharp;

namespace ConsoleExample.Demos;

/// <summary>A made-up callsign, and a full scripted contact between two stations.</summary>
internal static class CallsignAndQsoDemo
{
    public static void Run()
    {
        Console.WriteLine($"Random callsign -> {Callsign.Next()}");

        Console.WriteLine("Random QSO:");
        foreach (string transmission in Qso.Generate())
            Console.WriteLine($"  {transmission}");
    }
}
