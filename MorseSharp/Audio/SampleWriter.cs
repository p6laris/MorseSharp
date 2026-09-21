namespace MorseSharp.Audio;

/// <summary>
/// Writes an element stream straight into the output buffer: tones are block copies from a precomputed sine buffer
/// and gaps are cleared in place, so each output byte is touched exactly once.
/// </summary>
internal ref struct SampleWriter(Span<byte> destination, ReadOnlySpan<byte> toneBytes, SampleTiming timing) : IElementSink
{
    private readonly Span<byte> _destination = destination;
    private readonly ReadOnlySpan<byte> _toneBytes = toneBytes;
    private readonly SampleTiming _timing = timing;
    private int _position;

    /// <inheritdoc />
    public void Dot() => Tone(_timing.Dot);

    /// <inheritdoc />
    public void Dash() => Tone(_timing.Dash);

    /// <inheritdoc />
    public void ElementGap() => Silence(_timing.ElementGap);

    /// <inheritdoc />
    public void CharGap() => Silence(_timing.CharGap);

    /// <inheritdoc />
    public void WordGap() => Silence(_timing.WordGap);

    /// <summary>Throws when the written length does not match the counted length.</summary>
    public readonly void AssertFull()
    {
        if (_position != _destination.Length)
            throw new InvalidOperationException("Internal error: the sample count and the rendered length differ.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Tone(int samples)
    {
        int bytes = samples * WavHeader.BytesPerSample;
        _toneBytes[..bytes].CopyTo(_destination.Slice(_position, bytes));
        _position += bytes;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Silence(int samples)
    {
        int bytes = samples * WavHeader.BytesPerSample;
        _destination.Slice(_position, bytes).Clear();
        _position += bytes;
    }
}
