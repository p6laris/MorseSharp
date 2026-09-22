namespace MorseSharp;

/// <summary>
/// How a Koch lesson went.
/// </summary>
/// <param name="Correct">Characters copied correctly.</param>
/// <param name="Total">Characters sent, not counting the spaces between groups.</param>
public readonly record struct KochScore(int Correct, int Total)
{
    /// <summary>The fraction copied correctly, from 0 to 1. Zero for an empty lesson.</summary>
    public double Accuracy => Total == 0 ? 0 : (double)Correct / Total;

    /// <summary>Whether this clears <see cref="Koch.Threshold"/> and earns the next character.</summary>
    public bool ClearsThreshold => Accuracy >= Koch.Threshold;
}
