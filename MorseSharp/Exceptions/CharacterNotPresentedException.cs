namespace MorseSharp.Exceptions;

/// <summary>
/// Thrown when a character of the input text has no Morse code in the selected language.
/// </summary>
public sealed class CharacterNotPresentedException(char character, Language language)
    : Exception($"The character '{character}' (U+{(int)character:X4}) is not present in the {language} alphabet.")
{
    /// <summary>The offending character.</summary>
    public char Character { get; } = character;

    /// <summary>The language that was selected.</summary>
    public Language Language { get; } = language;
}
