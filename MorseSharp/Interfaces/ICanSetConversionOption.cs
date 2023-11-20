namespace MorseSharp.Interfaces
{
    /// <summary>
    /// Represents an interface that allows setting conversion options for Morse code.
    /// </summary>
    public interface ICanSetConversionOption
    {
        /// <summary>
        /// Converts Morse code to text.
        /// </summary>
        /// <param name="morse">The Morse code to be converted to text.</param>
        /// <returns>The converted text.</returns>
        string Decode(string morse);

        /// <summary>
        /// Converts text to Morse code.
        /// </summary>
        /// <param name="text">The text to be converted to Morse code.</param>
        /// <returns>The converted Morse code.</returns>
        ICanGenerateAudioAndLight ToMorse(string text);
        ICanSetAudioOptions ToAudio(string morse);
        ICanSetBlinkerOptions ToLight(string morse);
    }
}