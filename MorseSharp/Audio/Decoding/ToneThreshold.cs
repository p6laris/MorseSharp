namespace MorseSharp.Audio.Decoding;

/// <summary>
/// Finds the energy level that separates key-down blocks from key-up blocks.
/// </summary>
/// <remarks>
/// <para>
/// Morse energy is strongly bimodal: blocks are either tone or not, with little in between. That makes this the
/// classic two-class split, so it uses Otsu's method, which picks the level that best separates the two groups by
/// maximising the variance between them.
/// </para>
/// <para>
/// A level derived from the signal as a whole beats one adapted on the fly, because a decoder that starts cold has
/// no idea what silence sounds like yet and will mistake ordinary ripple in the first tone for keying.
/// </para>
/// </remarks>
internal static class ToneThreshold
{
    private const int Buckets = 256;

    /// <summary>
    /// Returns the separating energy level, or <see cref="float.MaxValue"/> when the input carries no tone at all.
    /// </summary>
    public static float Compute(ReadOnlySpan<float> powers)
    {
        if (powers.IsEmpty)
            return float.MaxValue;

        float min = float.MaxValue;
        float max = float.MinValue;
        foreach (float power in powers)
        {
            if (power < min) min = power;
            if (power > max) max = power;
        }

        // Flat input: silence throughout, or a continuous tone with nothing to separate.
        if (max - min <= float.Epsilon)
            return float.MaxValue;

        Span<int> histogram = stackalloc int[Buckets];
        histogram.Clear();
        float scale = (Buckets - 1) / (max - min);
        foreach (float power in powers)
            histogram[(int)((power - min) * scale)]++;

        float weightedTotal = 0;
        for (int i = 0; i < Buckets; i++)
            weightedTotal += (float)i * histogram[i];

        int belowCount = 0;
        float belowWeighted = 0;
        float bestVariance = -1f;
        int bestBucket = 0;

        for (int i = 0; i < Buckets; i++)
        {
            belowCount += histogram[i];
            if (belowCount == 0)
                continue;

            int aboveCount = powers.Length - belowCount;
            if (aboveCount == 0)
                break;

            belowWeighted += (float)i * histogram[i];
            float belowMean = belowWeighted / belowCount;
            float aboveMean = (weightedTotal - belowWeighted) / aboveCount;
            float difference = belowMean - aboveMean;
            float variance = (float)belowCount * aboveCount * difference * difference;

            if (variance > bestVariance)
            {
                bestVariance = variance;
                bestBucket = i;
            }
        }

        return min + ((bestBucket + 0.5f) * (max - min) / (Buckets - 1));
    }
}
