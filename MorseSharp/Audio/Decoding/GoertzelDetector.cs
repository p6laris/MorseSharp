namespace MorseSharp.Audio.Decoding;

/// <summary>
/// Measures how much energy a block of samples carries at one target frequency.
/// </summary>
/// <remarks>
/// <para>
/// A full FFT would report every frequency, and a Morse decoder only cares about one. Goertzel is the single-bin
/// case: a second-order resonator tuned to the target, costing one multiply and two adds per sample with no
/// buffers and no allocation.
/// </para>
/// <para>
/// The target is snapped to the nearest whole frequency bin for the given window. An off-bin target leaks energy
/// into neighbouring bins and reads low, so the snap keeps the measurement honest at the cost of landing a few
/// hertz away from what was asked for.
/// </para>
/// </remarks>
internal readonly struct GoertzelDetector
{
    private readonly float _coefficient;

    /// <summary>The frequency actually being measured, after snapping to a whole bin.</summary>
    public float Frequency { get; }

    /// <summary>Samples per analysis block.</summary>
    public int WindowSize { get; }

    /// <summary>Creates a detector.</summary>
    /// <param name="targetFrequency">Frequency of interest, in hertz.</param>
    /// <param name="sampleRate">Sample rate of the audio, in hertz.</param>
    /// <param name="windowSize">Samples per block. Larger is more selective but blurs short elements.</param>
    public GoertzelDetector(double targetFrequency, int sampleRate, int windowSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sampleRate);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(windowSize);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(targetFrequency);

        WindowSize = windowSize;

        int bin = (int)(0.5 + (windowSize * targetFrequency / sampleRate));
        double omega = 2.0 * Math.PI * bin / windowSize;
        _coefficient = (float)(2.0 * Math.Cos(omega));
        Frequency = (float)((double)bin * sampleRate / windowSize);
    }

    /// <summary>
    /// Returns the energy at the target frequency for one block of 16-bit PCM samples.
    /// </summary>
    /// <remarks>
    /// This is magnitude squared, not amplitude. Only its size relative to the surrounding noise matters here, so it
    /// is never converted back, which saves a square root per block.
    /// </remarks>
    public float Power(ReadOnlySpan<short> samples)
    {
        float coefficient = _coefficient;
        float s1 = 0f;
        float s2 = 0f;

        for (int i = 0; i < samples.Length; i++)
        {
            float sample = samples[i] * (1f / 32768f);
            float s0 = sample + (coefficient * s1) - s2;
            s2 = s1;
            s1 = s0;
        }

        float power = (s1 * s1) + (s2 * s2) - (coefficient * s1 * s2);
        return power > 0f ? power : 0f;
    }
}
