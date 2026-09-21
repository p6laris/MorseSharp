namespace MorseSharp.Interfaces;

/// <summary>
/// Step that configures the light-blink timing.
/// </summary>
public interface ICanSetBlinkerOptions
{
    /// <summary>
    /// Sets the Farnsworth timing.
    /// </summary>
    /// <param name="charSpeed">Speed at which individual characters are keyed, in words per minute (PARIS standard).</param>
    /// <param name="wordSpeed">Overall speed including stretched gaps, in words per minute. Must not exceed <paramref name="charSpeed"/>.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a speed is not positive.</exception>
    /// <exception cref="Exceptions.SmallerCharSpeedException">Thrown when <paramref name="charSpeed"/> is smaller than <paramref name="wordSpeed"/>.</exception>
    ICanConvertToLight SetBlinkerOptions(int charSpeed = 25, int wordSpeed = 25);
}
