using System.Text;

namespace MorseSharp.Audio.Decoding;

/// <summary>
/// Turns received audio back into text.
/// </summary>
/// <remarks>
/// <para>
/// Four stages. <see cref="GoertzelDetector"/> reports the energy at the operator's tone for each short block of
/// samples, <see cref="ToneThreshold"/> and <see cref="ToneGate"/> turn those energies into key-down and key-up runs,
/// this class measures the runs against a dit length it keeps adjusting, and the resulting dots and dashes are folded
/// into the same tree code the rest of the library uses, so the final lookup is one array index.
/// </para>
/// <para>
/// The energies are measured for the whole buffer before any of them is judged. A decoder that adapts as it goes has
/// heard no silence when the first tone arrives, so it has nothing to compare against and mistakes ordinary ripple
/// within that tone for keying, splitting the opening character.
/// </para>
/// <para>
/// The dit estimate matters more than anything else after that. Hand-sent Morse drifts, so a length fixed from the
/// stated speed decodes the first word and then falls apart. Every element seen nudges the estimate, which is why the
/// stated speed only has to be close rather than correct.
/// </para>
/// </remarks>
internal static class MorseAudioDecoder
{
    /// <summary>Blocks per dit to aim for. Enough to time an element, few enough to keep each block selective.</summary>
    private const int BlocksPerDit = 8;

    private const int MinWindow = 16;
    private const int MaxWindow = 512;

    /// <summary>How strongly each measured element pulls the dit estimate. Fast enough to track drift, slow enough to ignore a glitch.</summary>
    private const float Adapt = 0.20f;

    /// <summary>Boundary between a dot and a dash, in dits.</summary>
    private const float DashThreshold = 2f;

    /// <summary>Boundary between a gap inside a character and a gap between characters, in dits.</summary>
    private const float CharGapThreshold = 2f;

    /// <summary>
    /// Where a word break sits between the character gap and the word gap, which stand in a 3:7 ratio. Judging long
    /// gaps against each other rather than against the dit is what lets Farnsworth spacing decode, since there the
    /// gaps are stretched far beyond three dits while the characters stay fast.
    /// </summary>
    private const float WordGapFactor = 1.6f;

    /// <summary>How much of a block's energy must sit at the target frequency before it counts as a tone.</summary>
    private const float MinimumToneShare = 0.5f;

    /// <summary>Chooses an analysis block size that resolves a dit at the given speed.</summary>
    public static int WindowSizeFor(int sampleRate, int wordsPerMinute)
    {
        double ditSamples = MorseTiming.DitFactor / wordsPerMinute * sampleRate;
        int window = (int)(ditSamples / BlocksPerDit);
        return Math.Clamp(window, MinWindow, MaxWindow);
    }

    /// <summary>
    /// Decodes a buffer of 16-bit PCM mono samples.
    /// </summary>
    /// <param name="samples">The audio.</param>
    /// <param name="alphabet">Alphabet to resolve sequences against.</param>
    /// <param name="sampleRate">Sample rate of the audio, in hertz.</param>
    /// <param name="frequency">The tone frequency to listen for, in hertz.</param>
    /// <param name="wordsPerMinute">Roughly how fast the sender is keying. Refined as decoding proceeds.</param>
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
            GoertzelDetector detector = new(frequency, sampleRate, window);

            float fullScale = window / 2f * (window / 2f);
            float strongestTone = 0f;
            float strongestBlock = 0f;

            for (int i = 0; i < blocks; i++)
            {
                ReadOnlySpan<short> block = samples.Slice(i * window, window);
                powers[i] = detector.Power(block);

                // Track the tone against everything else in the block, to tell a real signal from mere leakage.
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

            // A tone holds a good share of the block's energy. A neighbouring frequency picking up spill does not,
            // and transcribing that spill would invent characters that were never sent.
            if (strongestBlock <= float.Epsilon || strongestTone / strongestBlock < MinimumToneShare)
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

    /// <summary>The starting dit estimate, in blocks, so run lengths can be compared against it directly.</summary>
    private static float DitBlocks(int sampleRate, int wordsPerMinute, int window)
    {
        float dit = (float)(MorseTiming.DitFactor / wordsPerMinute * sampleRate / window);
        return dit < 1f ? 1f : dit;
    }

    private static string Transcribe(ReadOnlySpan<float> powers, float threshold, MorseAlphabet alphabet, float statedDit)
    {
        int[] rented = ArrayPool<int>.Shared.Rent(powers.Length + 1);
        try
        {
            // Runs are recorded before any are judged: length carries the duration, the sign carries key up or down.
            Span<int> runs = rented;
            int count = CollectRuns(powers, threshold, runs);
            if (count == 0)
                return string.Empty;

            float dit = EstimateDit(runs[..count], statedDit);
            float wordGap = EstimateWordGap(runs[..count], dit);

            StringBuilder text = new();
            int code = MorseAlphabet.WordSpaceCode;
            for (int i = 0; i < count; i++)
            {
                int run = runs[i];
                if (run > 0)
                    AddSymbol(ref code, ref dit, run);
                else
                    AddGap(text, ref code, ref dit, wordGap, -run, alphabet);
            }

            Emit(text, ref code, alphabet);
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

    /// <summary>
    /// Estimates the dit length from the runs themselves rather than from the speed the caller stated.
    /// </summary>
    /// <remarks>
    /// Dots and the gaps between the symbols of a character are both exactly one dit, and in ordinary text they far
    /// outnumber the three and seven dit runs. The short end of the distribution is therefore made of dits. Taking
    /// the median of the shortest quarter reads that off while staying immune to a stray one-block run, and it means
    /// a caller who states the speed wrongly still gets a correct transcription.
    /// </remarks>
    private static float EstimateDit(ReadOnlySpan<int> runs, float statedDit)
    {
        int[] rented = ArrayPool<int>.Shared.Rent(runs.Length);
        try
        {
            Span<int> lengths = rented.AsSpan(0, runs.Length);
            for (int i = 0; i < runs.Length; i++)
                lengths[i] = Math.Abs(runs[i]);

            lengths.Sort();

            int quartile = Math.Max(1, lengths.Length / 4);
            int measured = lengths[quartile / 2];
            return measured >= 1 ? measured : statedDit;
        }
        finally
        {
            ArrayPool<int>.Shared.Return(rented);
        }
    }

    /// <summary>Classifies a key-down run as a dot or a dash and folds it into the tree code.</summary>
    private static void AddSymbol(ref int code, ref float dit, int blocks)
    {
        bool isDash = blocks > dit * DashThreshold;

        // A dash is three dits, so both kinds of element can inform the estimate.
        float impliedDit = isDash ? blocks / 3f : blocks;
        dit += (impliedDit - dit) * Adapt;
        if (dit < 1f)
            dit = 1f;

        if (code < MorseAlphabet.CodeLimit)
            code = isDash ? (code << 1) | 1 : code << 1;
    }

    /// <summary>Classifies a key-up run, emitting a character or a word break when the gap is long enough.</summary>
    private static void AddGap(StringBuilder text, ref int code, ref float dit, float wordGap, int blocks, MorseAlphabet alphabet)
    {
        if (blocks <= dit * CharGapThreshold)
        {
            // Inside a character. Short gaps are the most reliable dit measurement there is.
            dit += (blocks - dit) * Adapt;
            if (dit < 1f)
                dit = 1f;
            return;
        }

        Emit(text, ref code, alphabet);

        if (blocks >= wordGap)
            text.Append(' ');
    }

    /// <summary>
    /// Finds the length at which a gap stops separating characters and starts separating words.
    /// </summary>
    /// <remarks>
    /// Character gaps outnumber word gaps in ordinary text, so the median of the long gaps is the character gap. A
    /// word gap is 7:3 longer than that, and the split is placed part of the way up so either can be recognised
    /// whether or not the sender used Farnsworth spacing.
    /// </remarks>
    private static float EstimateWordGap(ReadOnlySpan<int> runs, float dit)
    {
        int[] rented = ArrayPool<int>.Shared.Rent(runs.Length);
        try
        {
            int count = 0;
            foreach (int run in runs)
            {
                if (run < 0 && -run > dit * CharGapThreshold)
                    rented[count++] = -run;
            }

            if (count == 0)
                return float.MaxValue;   // one word, so nothing is a word gap

            Span<int> gaps = rented.AsSpan(0, count);
            gaps.Sort();
            return gaps[count / 2] * WordGapFactor;
        }
        finally
        {
            ArrayPool<int>.Shared.Return(rented);
        }
    }

    /// <summary>Resolves the accumulated tree code and appends the character.</summary>
    private static void Emit(StringBuilder text, ref int code, MorseAlphabet alphabet)
    {
        if (code == MorseAlphabet.WordSpaceCode)
            return;

        char decoded = alphabet.Decode(code);
        text.Append(decoded == '\0' ? '?' : decoded);
        code = MorseAlphabet.WordSpaceCode;
    }
}
