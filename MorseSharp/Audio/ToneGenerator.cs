using System.Buffers.Binary;

namespace MorseSharp.Audio;

/// <summary>
/// Renders one keyed element as audio, already in the format it will be written in.
/// </summary>
/// <remarks>
/// <para>
/// No <see cref="Math.Sin(double)"/> call is made per sample. A unit vector is rotated by the phase increment using
/// the angle-addition identities, which costs a handful of multiplies and adds per sample, and the SIMD path rotates
/// several phases at once.
/// </para>
/// <para>
/// Each element fades in and out. Without that the waveform jumps from full amplitude to silence between one sample
/// and the next, and an instant jump like that spreads energy across the whole spectrum rather than staying at the
/// tone frequency. That is heard as a click at both ends of every dot and dash.
/// </para>
/// </remarks>
internal static class ToneGenerator
{
    /// <summary>Peak amplitude, just below full scale to leave headroom against rounding.</summary>
    public const double Amplitude = 32760.0 / 32768.0;

    /// <summary>
    /// Fills <paramref name="destination"/> with one element: a tone of <paramref name="samples"/> frames, faded in
    /// and out, interleaved across the channels and encoded at the requested depth.
    /// </summary>
    /// <param name="destination">Exactly <paramref name="samples"/> frames' worth of bytes.</param>
    /// <param name="samples">Length of the element, in frames.</param>
    /// <param name="frequency">Tone frequency in hertz.</param>
    /// <param name="format">How the audio is stored, and how long the fade lasts.</param>
    public static void Render(Span<byte> destination, int samples, double frequency, AudioFormat format)
    {
        double increment = 2 * Math.PI * frequency / format.SampleRate;

        // The fade cannot take more than half the element, or the two ends would overlap.
        int edge = (int)Math.Round(format.EdgeMilliseconds / 1000.0 * format.SampleRate);
        edge = Math.Clamp(edge, 0, samples / 2);

        double sin = 0;
        double cos = 1;
        double cosInc = Math.Cos(increment);
        double sinInc = Math.Sin(increment);

        int bytesPerSample = format.BytesPerSample;
        int position = 0;

        for (int i = 0; i < samples; i++)
        {
            double value = Amplitude * sin * Gain(i, samples, edge);

            for (int channel = 0; channel < format.Channels; channel++)
            {
                WriteSample(destination[position..], value, format.BitDepth);
                position += bytesPerSample;
            }

            double nextSin = sin * cosInc + cos * sinInc;
            cos = cos * cosInc - sin * sinInc;
            sin = nextSin;
        }
    }

    /// <summary>
    /// The envelope: a raised cosine rising over the first <paramref name="edge"/> samples and falling over the last,
    /// flat in between. Raised cosine rather than a straight line because its slope starts and ends at zero, so there
    /// is no corner anywhere for the ear to hear.
    /// </summary>
    private static double Gain(int index, int samples, int edge)
    {
        if (edge == 0)
            return 1.0;

        if (index < edge)
            return 0.5 - (0.5 * Math.Cos(Math.PI * index / edge));

        int fromEnd = samples - 1 - index;
        if (fromEnd < edge)
            return 0.5 - (0.5 * Math.Cos(Math.PI * fromEnd / edge));

        return 1.0;
    }

    private static void WriteSample(Span<byte> destination, double value, AudioBitDepth depth)
    {
        switch (depth)
        {
            case AudioBitDepth.Pcm8:
                // 8-bit WAV samples are unsigned, so silence sits at 128 rather than 0.
                destination[0] = (byte)Math.Clamp(128 + (value * 127), byte.MinValue, byte.MaxValue);
                break;

            case AudioBitDepth.Float32:
                BinaryPrimitives.WriteSingleLittleEndian(destination, (float)value);
                break;

            default:
                BinaryPrimitives.WriteInt16LittleEndian(destination, (short)Math.Clamp(value * short.MaxValue, short.MinValue, short.MaxValue));
                break;
        }
    }
}
