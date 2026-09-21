namespace MorseSharp.Audio;

/// <summary>
/// Fills a buffer with a sine wave of a given frequency, vectorised where the hardware allows it.
/// </summary>
/// <remarks>
/// <para>
/// No <see cref="Math.Sin(double)"/> call is made per sample. A unit vector is rotated by the phase increment using
/// the angle-addition identities, which costs four multiplies and two adds per sample.
/// </para>
/// <para>
/// The SIMD path rotates <see cref="Vector{T}"/> lanes that hold four consecutive blocks of phases and advances all of
/// them by the width of those blocks at once, then narrows the four <see cref="double"/> vectors down to a single
/// <see cref="short"/> vector in two steps. With 256-bit vectors that is sixteen samples per iteration.
/// </para>
/// </remarks>
internal static class ToneGenerator
{
    /// <summary>Peak amplitude, just below <see cref="short.MaxValue"/> to leave headroom against rounding.</summary>
    public const short Amplitude = 32760;

    /// <summary>
    /// Writes a sine wave of <paramref name="frequency"/> hertz, starting at phase zero, into <paramref name="tone"/>.
    /// </summary>
    /// <param name="tone">Destination buffer; every element is written.</param>
    /// <param name="frequency">Tone frequency in hertz.</param>
    /// <param name="sampleRate">Sample rate in hertz.</param>
    public static void Fill(Span<short> tone, double frequency, int sampleRate)
    {
        double increment = 2 * Math.PI * frequency / sampleRate;

        int written = Vector.IsHardwareAccelerated && Vector<double>.Count >= 2
            ? FillVectorized(tone, increment)
            : 0;

        FillScalar(tone[written..], increment, written);
    }

    /// <summary>Rotates four vectors of phases at a time and narrows them into one vector of samples.</summary>
    private static int FillVectorized(Span<short> tone, double increment)
    {
        int lanes = Vector<double>.Count;
        int block = 4 * lanes;              // doubles produced per iteration == Vector<short>.Count
        int iterations = tone.Length / block;
        if (iterations == 0)
            return 0;

        Span<double> seedSin = stackalloc double[Vector<short>.Count];
        Span<double> seedCos = stackalloc double[Vector<short>.Count];
        for (int i = 0; i < block; i++)
        {
            (seedSin[i], seedCos[i]) = Math.SinCos(i * increment);
        }

        Vector<double> sin0 = new(seedSin), cos0 = new(seedCos);
        Vector<double> sin1 = new(seedSin[lanes..]), cos1 = new(seedCos[lanes..]);
        Vector<double> sin2 = new(seedSin[(2 * lanes)..]), cos2 = new(seedCos[(2 * lanes)..]);
        Vector<double> sin3 = new(seedSin[(3 * lanes)..]), cos3 = new(seedCos[(3 * lanes)..]);

        // One iteration advances every lane by a whole block.
        (double stepSin, double stepCos) = Math.SinCos(block * increment);
        Vector<double> sinStep = new(stepSin);
        Vector<double> cosStep = new(stepCos);
        Vector<double> amplitude = new(Amplitude);

        Span<short> destination = tone;
        for (int i = 0; i < iterations; i++)
        {
            Vector<long> l0 = Vector.ConvertToInt64(sin0 * amplitude);
            Vector<long> l1 = Vector.ConvertToInt64(sin1 * amplitude);
            Vector<long> l2 = Vector.ConvertToInt64(sin2 * amplitude);
            Vector<long> l3 = Vector.ConvertToInt64(sin3 * amplitude);

            Vector.Narrow(Vector.Narrow(l0, l1), Vector.Narrow(l2, l3)).CopyTo(destination);
            destination = destination[block..];

            Rotate(ref sin0, ref cos0, sinStep, cosStep);
            Rotate(ref sin1, ref cos1, sinStep, cosStep);
            Rotate(ref sin2, ref cos2, sinStep, cosStep);
            Rotate(ref sin3, ref cos3, sinStep, cosStep);
        }

        return iterations * block;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Rotate(ref Vector<double> sin, ref Vector<double> cos, Vector<double> sinStep, Vector<double> cosStep)
    {
        Vector<double> nextSin = sin * cosStep + cos * sinStep;
        cos = cos * cosStep - sin * sinStep;
        sin = nextSin;
    }

    /// <summary>Fills the remainder that does not make up a whole vector, starting at sample <paramref name="offset"/>.</summary>
    private static void FillScalar(Span<short> tone, double increment, int offset)
    {
        if (tone.IsEmpty)
            return;

        (double sin, double cos) = Math.SinCos(offset * increment);
        (double sinInc, double cosInc) = Math.SinCos(increment);

        for (int i = 0; i < tone.Length; i++)
        {
            tone[i] = (short)(Amplitude * sin);
            double nextSin = sin * cosInc + cos * sinInc;
            cos = cos * cosInc - sin * sinInc;
            sin = nextSin;
        }
    }
}
