namespace MorseSharp.Audio;

/// <summary>
/// Element durations in samples at a given sample rate. The dash is exactly three dots so the dot waveform is a
/// prefix of the dash waveform and only one tone buffer is needed.
/// </summary>
internal readonly struct SampleTiming
{
    /// <summary>Samples in a dot.</summary>
    public readonly int Dot;

    /// <summary>Samples in a dash.</summary>
    public readonly int Dash;

    /// <summary>Samples in a gap between symbols.</summary>
    public readonly int ElementGap;

    /// <summary>Samples in a gap between characters.</summary>
    public readonly int CharGap;

    /// <summary>Samples in a gap between words.</summary>
    public readonly int WordGap;

    /// <summary>Rounds the durations of <paramref name="timing"/> to whole samples.</summary>
    public SampleTiming(in MorseTiming timing, int sampleRate)
    {
        Dot = Math.Max(1, (int)Math.Round(timing.Dot * sampleRate));
        Dash = 3 * Dot;
        ElementGap = Dot;
        CharGap = (int)Math.Round(timing.CharGap * sampleRate);
        WordGap = (int)Math.Round(timing.WordGap * sampleRate);
    }
}
