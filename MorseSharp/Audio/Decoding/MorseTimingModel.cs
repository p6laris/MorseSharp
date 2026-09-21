namespace MorseSharp.Audio.Decoding;

/// <summary>
/// Holds what the decoder currently believes about the sender's timing, and classifies runs against it.
/// </summary>
/// <remarks>
/// <para>
/// Morse carries no absolute durations, only ratios: a dash is three dits, and the gaps are one, three and seven.
/// Everything therefore follows from the dit, which is why this is where the real difficulty of decoding lives.
/// </para>
/// <para>
/// Long gaps are judged against each other rather than against the dit. Under Farnsworth spacing the characters stay
/// fast while the gaps are stretched far past three dits, so a rule tied to the dit would read every character gap as
/// a word break. The 3:7 ratio between character and word gaps survives the stretching, and that is what gets used.
/// </para>
/// </remarks>
internal struct MorseTimingModel
{
    /// <summary>How strongly each measured element pulls the dit estimate. Fast enough to track drift, slow enough to ignore a glitch.</summary>
    private const float Adapt = 0.20f;

    /// <summary>Boundary between a dot and a dash, in dits.</summary>
    public const float DashThreshold = 2f;

    /// <summary>Boundary between a gap inside a character and one between characters, in dits.</summary>
    public const float CharGapThreshold = 2f;

    /// <summary>Where a word break sits above the measured character gap, given their 3:7 ratio.</summary>
    public const float WordGapFactor = 1.6f;

    private float _dit;
    private float _wordGap;

    /// <summary>Creates a model seeded with a dit length in blocks.</summary>
    public MorseTimingModel(float dit, float wordGap)
    {
        _dit = dit < 1f ? 1f : dit;
        _wordGap = wordGap;
    }

    /// <summary>The current dit estimate, in blocks.</summary>
    public readonly float Dit => _dit;

    /// <summary>The length at which a gap becomes a word break, in blocks.</summary>
    public readonly float WordGap => _wordGap;

    /// <summary>Replaces the word-gap estimate, for a decoder that re-measures it as it goes.</summary>
    public void SetWordGap(float wordGap) => _wordGap = wordGap;

    /// <summary>Classifies a key-down run, and lets it refine the dit estimate.</summary>
    /// <returns><c>true</c> when the run is a dash.</returns>
    public bool ClassifyMark(int blocks)
    {
        bool isDash = blocks > _dit * DashThreshold;

        // A dash is three dits, so either kind of element can inform the estimate.
        Pull(isDash ? blocks / 3f : blocks);
        return isDash;
    }

    /// <summary>Classifies a key-up run.</summary>
    public MorseGap ClassifyGap(int blocks)
    {
        if (blocks <= _dit * CharGapThreshold)
        {
            // Inside a character. Short gaps are the most reliable dit measurement there is.
            Pull(blocks);
            return MorseGap.Element;
        }

        return blocks >= _wordGap ? MorseGap.Word : MorseGap.Character;
    }

    private void Pull(float impliedDit)
    {
        _dit += (impliedDit - _dit) * Adapt;
        if (_dit < 1f)
            _dit = 1f;
    }
}
