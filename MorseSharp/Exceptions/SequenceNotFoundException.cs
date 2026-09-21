namespace MorseSharp.Exceptions;

/// <summary>
/// Thrown when a Morse sequence has no character in the selected language.
/// </summary>
public sealed class SequenceNotFoundException(ReadOnlySpan<char> sequence, Language language)
    : Exception($"The Morse sequence '{sequence}' is not found in the {language} alphabet.")
{
    /// <summary>The offending sequence.</summary>
    public string Sequence { get; } = sequence.ToString();

    /// <summary>The language that was selected.</summary>
    public Language Language { get; } = language;
}
