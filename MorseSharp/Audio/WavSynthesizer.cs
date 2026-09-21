using System.Buffers.Binary;

namespace MorseSharp.Audio;

/// <summary>
/// Renders a Morse element stream as a 16-bit PCM mono WAV file.
/// </summary>
/// <remarks>
/// The output is produced in two passes over the source: the first pass counts samples so the exact size is known,
/// the second writes the header and then every sample exactly once. One precomputed sine buffer one dash long serves
/// both dots and dashes, so no intermediate audio buffers exist.
/// </remarks>
internal static class WavSynthesizer
{
    /// <summary>Output sample rate in hertz.</summary>
    public const int SampleRate = 11025;

    /// <summary>Size of the RIFF/WAVE header, in bytes.</summary>
    public const int HeaderSize = WavHeader.Size;

    /// <summary>Longest tone, in samples, that is built on the stack rather than from the array pool.</summary>
    private const int StackallocSampleLimit = 2048;

    /// <summary>Throws when the tone frequency cannot be represented at <see cref="SampleRate"/>.</summary>
    public static void ValidateFrequency(double frequency)
    {
        const double nyquist = SampleRate / 2.0;
        if (!(frequency > 0 && frequency < nyquist))
            throw new ArgumentOutOfRangeException(nameof(frequency), frequency, $"Frequency must be greater than 0 and less than {nyquist} Hz.");
    }

    /// <summary>Total WAV size in bytes for <paramref name="samples"/> PCM samples.</summary>
    public static int ByteCount(long samples)
    {
        long bytes = HeaderSize + samples * WavHeader.BytesPerSample;
        if (bytes > int.MaxValue)
            throw new InvalidOperationException("The audio is longer than 2 GB and cannot be stored in a single WAV file.");
        return (int)bytes;
    }

    /// <summary>Counts the samples <paramref name="source"/> produces.</summary>
    public static long CountSamples<TSource>(in SampleTiming timing, TSource source)
        where TSource : struct, IWalkSource
    {
        SampleCounter counter = new(timing);
        source.Walk(ref counter);
        return counter.Samples;
    }

    /// <summary>
    /// Renders <paramref name="source"/> into <paramref name="destination"/>, which must be exactly the size returned
    /// by <see cref="ByteCount"/> for the same source and timing.
    /// </summary>
    [SkipLocalsInit]
    public static void Render<TSource>(Span<byte> destination, in SampleTiming timing, double frequency, TSource source)
        where TSource : struct, IWalkSource
    {
        WavHeader.Write(destination, destination.Length - HeaderSize, SampleRate);

        short[]? rented = null;
        Span<short> tone = timing.Dash <= StackallocSampleLimit
            ? stackalloc short[StackallocSampleLimit]
            : (rented = ArrayPool<short>.Shared.Rent(timing.Dash));
        tone = tone[..timing.Dash];

        ToneGenerator.Fill(tone, frequency, SampleRate);

        // The samples are copied verbatim into a little-endian file.
        if (!BitConverter.IsLittleEndian)
            BinaryPrimitives.ReverseEndianness(tone, tone);

        SampleWriter writer = new(destination[HeaderSize..], MemoryMarshal.AsBytes(tone), timing);
        source.Walk(ref writer);
        writer.AssertFull();

        if (rented is not null)
            ArrayPool<short>.Shared.Return(rented);
    }
}
