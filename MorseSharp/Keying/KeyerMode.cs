namespace MorseSharp;

/// <summary>
/// How an iambic keyer behaves when a squeeze is released.
/// </summary>
/// <remarks>
/// Both modes finish the element they are sending. They differ only in what happens when both paddles are let go
/// during the gap that follows it: mode A stops there, mode B sends one more element of the opposite kind. Operators
/// learn one or the other and their timing depends on it, so this is a setting rather than a default worth arguing over.
/// </remarks>
public enum KeyerMode : byte
{
    /// <summary>Releasing the paddles ends the sequence after the element in progress.</summary>
    A = 0,

    /// <summary>Releasing the paddles adds one further element of the opposite kind.</summary>
    B = 1,
}
