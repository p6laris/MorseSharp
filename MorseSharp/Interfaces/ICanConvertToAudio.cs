namespace MorseSharp.Interfaces;

/// <summary>
/// Terminal step of the audio chain: produces a 16-bit PCM mono WAV file (11.025 kHz) for the current Morse sequence.
/// </summary>
public interface ICanConvertToAudio
{
    /// <summary>
    /// Returns the total size in bytes of the WAV file (44-byte header included) that the other members would produce.
    /// Use it to size a buffer for <see cref="GetBytes(Span{byte})"/>.
    /// </summary>
    int GetByteCount();

    /// <summary>
    /// Allocates and returns a complete WAV file as a byte array. The array is exactly sized; no copy is made.
    /// </summary>
    byte[] GetBytes();

    /// <summary>
    /// Writes a complete WAV file into <paramref name="destination"/> without allocating.
    /// </summary>
    /// <param name="destination">Buffer of at least <see cref="GetByteCount"/> bytes.</param>
    /// <returns>The number of bytes written.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="destination"/> is too small.</exception>
    int GetBytes(Span<byte> destination);

    /// <summary>
    /// Writes a complete WAV file to <paramref name="destination"/> using a pooled buffer.
    /// </summary>
    /// <param name="destination">A writable stream, for example a <see cref="FileStream"/> or <see cref="MemoryStream"/>.</param>
    void WriteTo(Stream destination);

    /// <summary>
    /// Allocates a WAV file and returns it as a span. Kept for source compatibility with 5.x.
    /// </summary>
    /// <param name="destination">Receives a span over a newly allocated array.</param>
    [Obsolete("Use GetBytes() (returns byte[]) or GetBytes(Span<byte>) to fill your own buffer. This overload allocates a new array on every call.")]
    void GetBytes(out Span<byte> destination);
}
