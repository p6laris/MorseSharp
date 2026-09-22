namespace MorseSharp;

/// <summary>
/// One step of a Morse sequence: whether the key is down, and for how long.
/// </summary>
/// <param name="Kind">Which of the five steps this is.</param>
/// <param name="Duration">How long it lasts at the timing it was produced with.</param>
public readonly record struct MorseElement(MorseElementKind Kind, TimeSpan Duration)
{
    /// <summary>
    /// <c>true</c> for a dot or a dash, <c>false</c> for any of the gaps. Drive a light, a buzzer or a relay with this.
    /// </summary>
    public bool KeyDown => Kind is MorseElementKind.Dot or MorseElementKind.Dash;
}
