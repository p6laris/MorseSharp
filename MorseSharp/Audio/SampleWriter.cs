namespace MorseSharp.Audio;

/// <summary>
/// Writes an element stream straight into the output buffer: tones are block copies from prerendered element
/// buffers and gaps are filled in place, so each output byte is touched exactly once.
/// </summary>
/// <remarks>
/// A dot used to be the first third of the dash buffer, which meant one buffer served both. Fading each element in
/// and out ends that: a dot's fade-out falls a third of the way into a dash, so the two are now different waveforms
/// and each is rendered once up front.
/// </remarks>
internal ref struct SampleWriter(
    Span<byte> destination,
    ReadOnlySpan<byte> dot,
    ReadOnlySpan<byte> dash,
    SampleTiming timing,
    int bytesPerFrame,
    byte silence) : IElementSink
{
    private readonly Span<byte> _destination = destination;
    private readonly ReadOnlySpan<byte> _dot = dot;
    private readonly ReadOnlySpan<byte> _dash = dash;
    private readonly SampleTiming _timing = timing;
    private readonly int _bytesPerFrame = bytesPerFrame;
    private readonly byte _silence = silence;
    private int _position;

    /// <inheritdoc />
    public void Dot() => Copy(_dot);

    /// <inheritdoc />
    public void Dash() => Copy(_dash);

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
    private void Copy(ReadOnlySpan<byte> element)
    {
        element.CopyTo(_destination.Slice(_position, element.Length));
        _position += element.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Silence(int samples)
    {
        int bytes = samples * _bytesPerFrame;
        _destination.Slice(_position, bytes).Fill(_silence);
        _position += bytes;
    }
}
