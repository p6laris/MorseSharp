namespace MorseSharp.Interfaces
{
    /// <summary>
    /// Represents an interface that allows specifying audio options for Morse code conversion.
    /// </summary>
    public interface ICanSetAudioOptions
    {
        /// <summary>
        /// Sets audio options for Morse code conversion.
        /// </summary>
        /// <param name="charSpeed">The character speed in words per minute (WPM).</param>
        /// <param name="wordSpeed">The word speed in words per minute (WPM).</param>
        /// <param name="frequency">The frequency (in Hertz) of the audio signal.</param>
        /// <returns>An object that allows further conversion to audio with the specified options.</returns>
        ICanConvertToAudio SetAudioOptions(int charSpeed, int wordSpeed, double frequency);
    }
}