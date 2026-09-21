using System.Text;

namespace MorseSharp.Audio.Decoding;

/// <summary>
/// Decodes a complete recording back to text.
/// </summary>
/// <remarks>
/// <para>
/// Four stages. <see cref="GoertzelDetector"/> reports the energy at the operator's tone for each short block of
/// samples, <see cref="ToneThreshold"/> finds the level separating key-down from key-up, <see cref="MorseTimingModel"/>
/// measures the resulting runs against a dit length taken from the signal, and <see cref="MorseSymbolAccumulator"/>
/// resolves the dots and dashes through the same tree code the encoder uses.
/// </para>
/// <para>
/// Having the whole recording is an advantage worth using. Every block is measured before any is judged, because a
/// decoder that adapts as it goes has heard no silence when the first tone arrives and mistakes ordinary ripple within
/// that tone for keying. <see cref="StreamingMorseDecoder"/> cannot do this and works harder for the same result.
/// </para>
/// </remarks>
internal static class MorseAudioDecoder
{
    /// <summary>Blocks per dit to aim for. Enough to time an element, few enough to keep each block selective.</summary>
    private const int BlocksPerDit = 8;

    private const int MinWindow = 16;
    private const int MaxWindow = 512;

    /// <summary>How much of a block's energy must sit at the target frequency before it counts as a tone.</summary>
    private const float MinimumToneShare = 0.5f;

    /// <summary>Chooses an analysis block size that resolves a dit at the given speed.</summary>
    public static int WindowSizeFor(int sampleRate, int wordsPerMinute)
    {
        double ditSamples = MorseTiming.DitFactor / wordsPerMinute * sampleRate;
        int window = (int)(ditSamples / BlocksPerDit);
        return Math.Clamp(window, MinWindow, MaxWindow);
    }

    /// <summary>The starting dit estimate, in blocks, so run lengths can be compared against it directly.</summary>
    public static float DitBlocks(int sampleRate, int wordsPerMinute, int window)
    {
        float dit = (float)(MorseTiming.DitFactor / wordsPerMinute * sampleRate / window);
        return dit < 1f ? 1f : dit;
    }

    /// <summary>
    /// Decodes a buffer of 16-bit PCM mono samples.
    /// </summary>
    /// <param name="samples">The audio.</param>
    /// <param name="alphabet">Alphabet to resolve sequences against.</param>
    /// <param name="sampleRate">Sample rate of the audio, in hertz.</param>
    /// <param name="frequency">The tone frequency to listen for, in hertz.</param>
    /// <param name="wordsPerMinute">Roughly how fast the sender is keying. Refined from the signal as decoding proceeds.</param>
    /// <returns>The decoded text. Sequences with no character in the alphabet decode to <c>?</c>.</returns>
    public static string Decode(
        ReadOnlySpan<short> samples,
        MorseAlphabet alphabet,
        int sampleRate,
        double frequency,
        int wordsPerMinute)
    {
        int window = WindowSizeFor(sampleRate, wordsPerMinute);
        int blocks = samples.Length / window;
        if (blocks == 0)
            return string.Empty;

        float[] rented = ArrayPool<float>.Shared.Rent(blocks);
        try
        {
            Span<float> powers = rented.AsSpan(0, blocks);
            if (!Measure(samples, powers, frequency, sampleRate, window))
                return string.Empty;

            float threshold = ToneThreshold.Compute(powers);
            if (threshold == float.MaxValue)
                return string.Empty;

            return Transcribe(powers, threshold, alphabet, DitBlocks(sampleRate, wordsPerMinute, window));
        }
        finally
        {
            ArrayPool<float>.Shared.Return(rented);
        }
    }

    /// <summary>
    /// Fills <paramref name="powers"/> with the per-block tone energy.
    /// </summary>
    /// <returns>
    /// <c>false</c> when the requested frequency carries no real tone. A neighbouring frequency picks up enough spill
    /// to follow the keying, and transcribing that spill would invent characters that were never sent.
    /// </returns>
    private static bool Measure(ReadOnlySpan<short> samples, Span<float> powers, double frequency, int sampleRate, int window)
    {
        GoertzelDetector detector = new(frequency, sampleRate, window);
        float fullScale = window / 2f * (window / 2f);
        float strongestTone = 0f;
        float strongestBlock = 0f;

        for (int i = 0; i < powers.Length; i++)
        {
            ReadOnlySpan<short> block = samples.Slice(i * window, window);
            powers[i] = detector.Power(block);

            float tone = powers[i] / fullScale;
            if (tone > strongestTone)
                strongestTone = tone;

            float total = 0f;
            foreach (short sample in block)
            {
                float normalised = sample * (1f / 32768f);
                total += normalised * normalised;
            }

            total /= window;
            if (total > strongestBlock)
                strongestBlock = total;
        }

        return strongestBlock > float.Epsilon && strongestTone / strongestBlock >= MinimumToneShare;
    }

    private static string Transcribe(ReadOnlySpan<float> powers, float threshold, MorseAlphabet alphabet, float seedDit)
    {
        int[] rented = ArrayPool<int>.Shared.Rent(powers.Length + 1);
        try
        {
            // Runs are recorded before any are judged: length carries the duration, the sign carries key up or down.
            Span<int> runs = rented;
            int count = CollectRuns(powers, threshold, runs);
            if (count == 0)
                return string.Empty;

            ReadOnlySpan<int> measured = runs[..count];
            float dit = RunStatistics.EstimateDit(measured, seedDit);
            MorseTimingModel timing = new(dit, RunStatistics.EstimateWordGap(measured, dit));
            MorseSymbolAccumulator symbols = new();

            StringBuilder text = new();
            foreach (int run in measured)
            {
                if (run > 0)
                {
                    symbols.Add(timing.ClassifyMark(run));
                    continue;
                }

                MorseGap gap = timing.ClassifyGap(-run);
                if (gap == MorseGap.Element)
                    continue;

                if (symbols.TryResolve(alphabet, out char character))
                    text.Append(character);

                if (gap == MorseGap.Word)
                    text.Append(' ');
            }

            if (symbols.TryResolve(alphabet, out char last))
                text.Append(last);

            return text.ToString().Trim();
        }
        finally
        {
            ArrayPool<int>.Shared.Return(rented);
        }
    }

    /// <summary>Walks the gate and records each unbroken stretch, positive for key down and negative for key up.</summary>
    private static int CollectRuns(ReadOnlySpan<float> powers, float threshold, Span<int> runs)
    {
        ToneGate gate = new(threshold);
        bool keyDown = false;
        int runLength = 0;
        int count = 0;

        foreach (float power in powers)
        {
            bool nowDown = gate.Update(power);
            if (nowDown == keyDown)
            {
                runLength++;
                continue;
            }

            if (runLength > 0)
                runs[count++] = keyDown ? runLength : -runLength;

            keyDown = nowDown;
            runLength = 1;
        }

        if (runLength > 0)
            runs[count++] = keyDown ? runLength : -runLength;

        return count;
    }
}
