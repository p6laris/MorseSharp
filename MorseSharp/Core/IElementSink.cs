namespace MorseSharp.Core;

/// <summary>
/// Receives the keyed elements of a Morse sequence in order. Implemented by small structs so the JIT can inline
/// each sink into the walker.
/// </summary>
internal interface IElementSink
{
    /// <summary>Key down for one unit.</summary>
    void Dot();

    /// <summary>Key down for three units.</summary>
    void Dash();

    /// <summary>Key up for one unit, between the symbols of a character.</summary>
    void ElementGap();

    /// <summary>Key up between two characters (three units, or stretched by Farnsworth timing).</summary>
    void CharGap();

    /// <summary>Key up between two words (seven units, or stretched by Farnsworth timing).</summary>
    void WordGap();
}
