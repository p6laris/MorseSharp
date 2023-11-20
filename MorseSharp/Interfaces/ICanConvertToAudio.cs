namespace MorseSharp.Interfaces
{
    /// <summary>
    /// Represents an interface that allows conversion to audio.
    /// </summary>
    public interface ICanConvertToAudio
    {
        /// <summary>
        /// Retrieves the audio data as a span of bytes.
        /// </summary>
        /// <param name="destination">An output span where the audio data will be stored.</param>
        void GetBytes(out Span<byte> destination);
    }
}