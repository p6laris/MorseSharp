namespace MorseSharp.Audio.Decoding;

/// <summary>
/// Reads the sender's timing out of the run lengths themselves.
/// </summary>
/// <remarks>
/// Measuring beats being told. A caller states a speed from memory or habit, and a live operator drifts anyway, so
/// the signal is the only trustworthy source. Both decoders use these, one over a whole recording and the other over
/// a sliding window of what it has heard lately.
/// </remarks>
internal static class RunStatistics
{
    /// <summary>
    /// Estimates the dit length, in blocks, from signed run lengths.
    /// </summary>
    /// <remarks>
    /// Dots and the gaps between the symbols of a character are both exactly one dit, and in ordinary text they far
    /// outnumber the three and seven dit runs, so the short end of the distribution is made of dits. Taking the median
    /// of the shortest quarter reads that off while staying immune to a stray one-block run.
    /// </remarks>
    /// <param name="runs">Run lengths, positive for key down and negative for key up.</param>
    /// <param name="fallback">Used when nothing usable can be measured.</param>
    public static float EstimateDit(ReadOnlySpan<int> runs, float fallback)
    {
        if (runs.IsEmpty)
            return fallback;

        int[] rented = ArrayPool<int>.Shared.Rent(runs.Length);
        try
        {
            Span<int> lengths = rented.AsSpan(0, runs.Length);
            for (int i = 0; i < runs.Length; i++)
                lengths[i] = Math.Abs(runs[i]);

            lengths.Sort();

            int quartile = Math.Max(1, lengths.Length / 4);
            int measured = lengths[quartile / 2];
            return measured >= 1 ? measured : fallback;
        }
        finally
        {
            ArrayPool<int>.Shared.Return(rented);
        }
    }

    /// <summary>
    /// Estimates the length at which a gap stops separating characters and starts separating words.
    /// </summary>
    /// <remarks>
    /// Character gaps outnumber word gaps in ordinary text, so the median of the long gaps is the character gap. A
    /// word gap is 7:3 longer, and the split is placed part of the way up, which works whether or not the sender
    /// stretched the spacing.
    /// </remarks>
    /// <returns><see cref="float.MaxValue"/> when no long gaps were seen, meaning nothing is a word break yet.</returns>
    public static float EstimateWordGap(ReadOnlySpan<int> runs, float dit)
    {
        if (runs.IsEmpty)
            return float.MaxValue;

        int[] rented = ArrayPool<int>.Shared.Rent(runs.Length);
        try
        {
            int count = 0;
            foreach (int run in runs)
            {
                if (run < 0 && -run > dit * MorseTimingModel.CharGapThreshold)
                    rented[count++] = -run;
            }

            if (count == 0)
                return float.MaxValue;

            Span<int> gaps = rented.AsSpan(0, count);
            gaps.Sort();
            return gaps[count / 2] * MorseTimingModel.WordGapFactor;
        }
        finally
        {
            ArrayPool<int>.Shared.Return(rented);
        }
    }
}
