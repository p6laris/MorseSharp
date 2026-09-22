using MorseSharp;

namespace ConsoleExample.Demos;

/// <summary>A Koch-method lesson, scored against itself to show what a perfect copy looks like.</summary>
internal static class KochDemo
{
    public static void Run()
    {
        string lesson = Koch.Generate(level: 5, groups: 6, random: new Random(1));
        Console.WriteLine($"Koch lesson (level 5) -> {lesson}");

        KochScore score = Koch.Score(lesson, lesson);
        Console.WriteLine($"Scored against itself -> {score.Accuracy:P0}, clears threshold: {score.ClearsThreshold}");
    }
}
