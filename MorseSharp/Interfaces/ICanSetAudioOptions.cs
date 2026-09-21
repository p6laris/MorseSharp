namespace MorseSharp.Interfaces;

/// <summary>
/// Step that configures the audio timing and tone.
/// </summary>
public interface ICanSetAudioOptions
{
    /// <summary>
    /// Sets the Farnsworth timing and the tone frequency.
    /// </summary>
    /// <param name="charSpeed">Speed at which individual characters are keyed, in words per minute (PARIS standard).</param>
    /// <param name="wordSpeed">Overall speed including stretched gaps, in words per minute. Must not exceed <paramref name="charSpeed"/>.</param>
    /// <param name="frequency">Tone frequency in hertz. Must be positive and below half the sample rate (5512.5 Hz).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when a speed is not positive or the frequency is out of range.</exception>
    /// <exception cref="Exceptions.SmallerCharSpeedException">Thrown when <paramref name="charSpeed"/> is smaller than <paramref name="wordSpeed"/>.</exception>
    ICanConvertToAudio SetAudioOptions(int charSpeed = 25, int wordSpeed = 25, double frequency = 700);
}
