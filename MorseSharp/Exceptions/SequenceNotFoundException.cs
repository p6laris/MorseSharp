namespace MorseSharp.Exceptions;

/// <summary>
/// Thrown when a Morse sequence has no character in the selected alphabet.
/// </summary>
public sealed class SequenceNotFoundException(ReadOnlySpan<char> sequence, string alphabetName)
    : Exception($"The Morse sequence '{sequence}' is not found in the {alphabetName} alphabet.")
{
    /// <summary>The offending sequence.</summary>
    public string Sequence { get; } = sequence.ToString();

    /// <summary>The name of the alphabet that was selected.</summary>
    public string AlphabetName { get; } = alphabetName;
}
