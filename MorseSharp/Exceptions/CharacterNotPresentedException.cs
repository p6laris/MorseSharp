namespace MorseSharp.Exceptions;

/// <summary>
/// Thrown when a character of the input text has no Morse code in the selected alphabet.
/// </summary>
public sealed class CharacterNotPresentedException(char character, string alphabetName)
    : Exception($"The character '{character}' (U+{(int)character:X4}) is not present in the {alphabetName} alphabet.")
{
    /// <summary>The offending character.</summary>
    public char Character { get; } = character;

    /// <summary>The name of the alphabet that was selected.</summary>
    public string AlphabetName { get; } = alphabetName;
}
