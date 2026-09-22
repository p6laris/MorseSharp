namespace MorseSharp.Audio;

/// <summary>
/// Element durations in seconds for a character speed / word speed pair (Farnsworth timing).
/// </summary>
/// <remarks>
/// Uses the ARRL definition: with <c>c</c> = character speed and <c>w</c> = word speed (both in PARIS words per minute),
/// a dit lasts <c>1.2 / c</c> seconds and the extra time to distribute over the gaps of one PARIS word is
/// <c>ta = 60 / w - 37.2 / c</c>. That time is split 3:7 between the four character gaps and the one word gap of PARIS,
/// so a character gap lasts <c>3 ta / 19</c> and a word gap <c>7 ta / 19</c>. When <c>c == w</c> this collapses to the
/// standard 3 and 7 dit gaps.
/// </remarks>
internal readonly struct MorseTiming
{
    /// <summary>Standard PARIS dit length factor: a dit lasts 1.2 / wpm seconds.</summary>
    public const double DitFactor = 1.2;

    /// <summary>Time in seconds consumed by the 31 keyed units of PARIS at 1 wpm.</summary>
    public const double ParisKeyedSeconds = 37.2;

    /// <summary>Duration of a dot.</summary>
    public readonly double Dot;

    /// <summary>Duration of a dash (three dots).</summary>
    public readonly double Dash;

    /// <summary>Gap between the symbols of one character (one dot).</summary>
    public readonly double ElementGap;

    /// <summary>Gap between two characters.</summary>
    public readonly double CharGap;

    /// <summary>Gap between two words.</summary>
    public readonly double WordGap;

    /// <summary>Computes the durations for a speed pair.</summary>
    public MorseTiming(int charSpeed, int wordSpeed)
    {
        Validate(charSpeed, wordSpeed);

        double dit = DitFactor / charSpeed;
        double stretch = 60.0 / wordSpeed - ParisKeyedSeconds / charSpeed;

        Dot = dit;
        Dash = 3 * dit;
        ElementGap = dit;
        CharGap = 3 * stretch / 19;
        WordGap = 7 * stretch / 19;
    }

    /// <summary>Duration in seconds of one element of the given kind.</summary>
    public double For(MorseElementKind kind) => kind switch
    {
        MorseElementKind.Dot => Dot,
        MorseElementKind.Dash => Dash,
        MorseElementKind.ElementGap => ElementGap,
        MorseElementKind.CharGap => CharGap,
        _ => WordGap,
    };

    /// <summary>Throws for non-positive speeds or a word speed above the character speed.</summary>
    public static void Validate(int charSpeed, int wordSpeed)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(charSpeed);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(wordSpeed);
        if (charSpeed < wordSpeed)
            throw new SmallerCharSpeedException(charSpeed, wordSpeed);
    }
}
