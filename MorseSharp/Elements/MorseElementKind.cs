namespace MorseSharp;

/// <summary>
/// What a single step of a Morse sequence is.
/// </summary>
public enum MorseElementKind : byte
{
    /// <summary>Key down for one unit.</summary>
    Dot,

    /// <summary>Key down for three units.</summary>
    Dash,

    /// <summary>Key up between the symbols of one character.</summary>
    ElementGap,

    /// <summary>Key up between two characters.</summary>
    CharGap,

    /// <summary>Key up between two words.</summary>
    WordGap,
}
