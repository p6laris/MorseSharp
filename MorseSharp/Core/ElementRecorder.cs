namespace MorseSharp.Core;

/// <summary>
/// Records elements as one byte each, so a sequence can be replayed later in real time without re-walking the input.
/// </summary>
internal ref struct ElementRecorder(Span<byte> destination) : IElementSink
{
    /// <summary>Key down for one unit.</summary>
    public const byte DotElement = 0;

    /// <summary>Key down for three units.</summary>
    public const byte DashElement = 1;

    /// <summary>Key up between the symbols of a character.</summary>
    public const byte ElementGapElement = 2;

    /// <summary>Key up between two characters.</summary>
    public const byte CharGapElement = 3;

    /// <summary>Key up between two words.</summary>
    public const byte WordGapElement = 4;

    private readonly Span<byte> _destination = destination;
    private int _count;

    /// <summary>Number of elements written.</summary>
    public readonly int Count => _count;

    /// <inheritdoc />
    public void Dot() => _destination[_count++] = DotElement;

    /// <inheritdoc />
    public void Dash() => _destination[_count++] = DashElement;

    /// <inheritdoc />
    public void ElementGap() => _destination[_count++] = ElementGapElement;

    /// <inheritdoc />
    public void CharGap() => _destination[_count++] = CharGapElement;

    /// <inheritdoc />
    public void WordGap() => _destination[_count++] = WordGapElement;
}
