namespace MorseSharp.Exceptions;

/// <summary>
/// Thrown when the character speed is smaller than the word speed. Farnsworth timing can only stretch gaps, never compress them.
/// </summary>
public sealed class SmallerCharSpeedException(int charSpeed, int wordSpeed)
    : Exception($"The character speed must not be smaller than the word speed: {charSpeed} < {wordSpeed}.")
{
    /// <summary>The character speed in words per minute.</summary>
    public int CharSpeed { get; } = charSpeed;

    /// <summary>The word speed in words per minute.</summary>
    public int WordSpeed { get; } = wordSpeed;
}
