namespace MorseSharp.Audio;

/// <summary>Adds up the samples an element stream will occupy, so the output can be sized exactly.</summary>
internal struct SampleCounter(SampleTiming timing) : IElementSink
{
    private readonly SampleTiming _timing = timing;

    /// <summary>Samples counted so far.</summary>
    public long Samples;

    /// <inheritdoc />
    public void Dot() => Samples += _timing.Dot;

    /// <inheritdoc />
    public void Dash() => Samples += _timing.Dash;

    /// <inheritdoc />
    public void ElementGap() => Samples += _timing.ElementGap;

    /// <inheritdoc />
    public void CharGap() => Samples += _timing.CharGap;

    /// <inheritdoc />
    public void WordGap() => Samples += _timing.WordGap;
}
