using System.Buffers.Binary;

namespace MorseSharp.Audio;

/// <summary>
/// The canonical 44-byte RIFF/WAVE header.
/// </summary>
internal static class WavHeader
{
    /// <summary>Size of the header in bytes.</summary>
    public const int Size = 44;

    private const ushort FormatPcm = 1;
    private const ushort FormatFloat = 3;

    /// <summary>Writes the header for a data chunk of <paramref name="dataBytes"/> bytes.</summary>
    /// <param name="destination">Buffer of at least <see cref="Size"/> bytes.</param>
    /// <param name="dataBytes">Size of the audio data that follows the header.</param>
    /// <param name="format">The format the data was written in.</param>
    public static void Write(Span<byte> destination, int dataBytes, AudioFormat format)
    {
        int bytesPerFrame = format.BytesPerFrame;
        ushort formatTag = format.BitDepth == AudioBitDepth.Float32 ? FormatFloat : FormatPcm;

        "RIFF"u8.CopyTo(destination);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[4..], (uint)(Size - 8 + dataBytes));
        "WAVE"u8.CopyTo(destination[8..]);

        "fmt "u8.CopyTo(destination[12..]);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[16..], 16);                                  // chunk size
        BinaryPrimitives.WriteUInt16LittleEndian(destination[20..], formatTag);
        BinaryPrimitives.WriteUInt16LittleEndian(destination[22..], (ushort)format.Channels);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[24..], (uint)format.SampleRate);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[28..], (uint)(format.SampleRate * bytesPerFrame));
        BinaryPrimitives.WriteUInt16LittleEndian(destination[32..], (ushort)bytesPerFrame);               // block align
        BinaryPrimitives.WriteUInt16LittleEndian(destination[34..], (ushort)(8 * format.BytesPerSample));

        "data"u8.CopyTo(destination[36..]);
        BinaryPrimitives.WriteUInt32LittleEndian(destination[40..], (uint)dataBytes);
    }
}
