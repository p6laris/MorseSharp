namespace MorseSharp.Interfaces
{
    public interface ICanGenerateAudioAndLight
    {

        string Encode();
        /// <summary>
        /// Converts text to audio with specified options.
        /// </summary>
        /// <param name="text">The text to be converted to audio.</param>
        /// <returns>An object that allows specifying audio options for the conversion.</returns>
        ICanSetAudioOptions ToAudio();

        ICanSetBlinkerOptions ToLight();

    }
}