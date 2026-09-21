namespace MorseSharp.Core;

/// <summary>Counts how many elements a sequence has, to size a recording buffer.</summary>
internal struct ElementCounter : IElementSink
{
    /// <summary>Number of elements seen so far.</summary>
    public int Count;

    /// <inheritdoc />
    public void Dot() => Count++;

    /// <inheritdoc />
    public void Dash() => Count++;

    /// <inheritdoc />
    public void ElementGap() => Count++;

    /// <inheritdoc />
    public void CharGap() => Count++;

    /// <inheritdoc />
    public void WordGap() => Count++;
}
