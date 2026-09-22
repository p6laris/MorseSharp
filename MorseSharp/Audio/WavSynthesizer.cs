namespace MorseSharp.Audio;

/// <summary>
/// Renders a Morse element stream as a WAV file.
/// </summary>
/// <remarks>
/// The output is produced in two passes over the source: the first counts samples so the exact size is known, the
/// second writes the header and then every sample exactly once. The two element waveforms are rendered up front in
/// the target format, so writing the body is block copies and fills with nothing to convert.
/// </remarks>
internal static class WavSynthesizer
{
    /// <summary>The sample rate used when the caller does not choose one.</summary>
    public const int SampleRate = 11025;

    /// <summary>Size of the RIFF/WAVE header, in bytes.</summary>
    public const int HeaderSize = WavHeader.Size;

    /// <summary>Throws when the tone frequency cannot be represented at the given sample rate.</summary>
    public static void ValidateFrequency(double frequency, int sampleRate)
    {
        double nyquist = sampleRate / 2.0;
        if (!(frequency > 0 && frequency < nyquist))
            throw new ArgumentOutOfRangeException(nameof(frequency), frequency, $"Frequency must be greater than 0 and less than {nyquist} Hz.");
    }

    /// <summary>Total WAV size in bytes for <paramref name="samples"/> frames.</summary>
    public static int ByteCount(long samples, AudioFormat format)
    {
        long bytes = HeaderSize + (samples * format.BytesPerFrame);
        if (bytes > int.MaxValue)
            throw new InvalidOperationException("The audio is longer than 2 GB and cannot be stored in a single WAV file.");
        return (int)bytes;
    }

    /// <summary>Counts the frames <paramref name="source"/> produces.</summary>
    public static long CountSamples<TSource>(in SampleTiming timing, TSource source)
        where TSource : struct, IWalkSource
    {
        SampleCounter counter = new(timing);
        source.Walk(ref counter);
        return counter.Samples;
    }

    /// <summary>
    /// Renders <paramref name="source"/> into <paramref name="destination"/>, which must be exactly the size returned
    /// by <see cref="ByteCount"/> for the same source, timing and format.
    /// </summary>
    public static void Render<TSource>(
        Span<byte> destination,
        in SampleTiming timing,
        double frequency,
        AudioFormat format,
        TSource source)
        where TSource : struct, IWalkSource
    {
        WavHeader.Write(destination, destination.Length - HeaderSize, format);

        int frameBytes = format.BytesPerFrame;
        int dotBytes = timing.Dot * frameBytes;
        int dashBytes = timing.Dash * frameBytes;

        byte[] elements = ArrayPool<byte>.Shared.Rent(dotBytes + dashBytes);
        try
        {
            Span<byte> dot = elements.AsSpan(0, dotBytes);
            Span<byte> dash = elements.AsSpan(dotBytes, dashBytes);

            ToneGenerator.Render(dot, timing.Dot, frequency, format);
            ToneGenerator.Render(dash, timing.Dash, frequency, format);

            SampleWriter writer = new(destination[HeaderSize..], dot, dash, timing, frameBytes, format.SilenceByte);
            source.Walk(ref writer);
            writer.AssertFull();
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(elements);
        }
    }
}
