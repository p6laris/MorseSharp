using System.Buffers.Binary;

namespace MorseSharp.Audio;

/// <summary>
/// The canonical 44-byte RIFF/WAVE header for uncompressed PCM.
/// </summary>
internal static class WavHeader
{
    /// <summary>Size of the header in bytes.</summary>
    public const int Size = 44;

    /// <summary>Bytes per PCM sample (16-bit mono).</summary>
    public const int BytesPerSample = 2;

    /// <summary>Writes the header for a data chunk of <paramref name="dataBytes"/> bytes.</summary>
    /// <param name="destination">Buffer of at least <see cref="Size"/> bytes.</param>
    /// <param name="dataBytes">Size of the PCM data that follows the header.</param>
    /// <param name="sampleRate">Sample rate in hertz.</param>
    public static void Write(Span<byte> destination, int dataBytes, int sampleRate)
    {
        "RIFF"u8.CopyTo(destination);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[4..], (uint)(Size - 8 + dataBytes));
        "WAVE"u8.CopyTo(destination[8..]);

        "fmt "u8.CopyTo(destination[12..]);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[16..], 16);                          // PCM format chunk size
        BinaryPrimitives.WriteUInt16LittleEndian(destination[20..], 1);                           // PCM, uncompressed
        BinaryPrimitives.WriteUInt16LittleEndian(destination[22..], 1);                           // mono
        BinaryPrimitives.WriteUInt32LittleEndian(destination[24..], (uint)sampleRate);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[28..], (uint)(sampleRate * BytesPerSample)); // byte rate
        BinaryPrimitives.WriteUInt16LittleEndian(destination[32..], BytesPerSample);              // block align
        BinaryPrimitives.WriteUInt16LittleEndian(destination[34..], 8 * BytesPerSample);          // bits per sample

        "data"u8.CopyTo(destination[36..]);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[40..], (uint)dataBytes);
    }
}
