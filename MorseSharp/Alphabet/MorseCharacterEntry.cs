namespace MorseSharp;

/// <summary>A character an alphabet maps, and the dot/dash pattern it is keyed as.</summary>
/// <remarks>Returned by <see cref="MorseAlphabet.Characters"/>; there is one per character the alphabet defines.</remarks>
public readonly struct MorseCharacterEntry
{
    internal MorseCharacterEntry(char character, string pattern, bool isAlias)
    {
        Character = character;
        Pattern = pattern;
        IsAlias = isAlias;
    }

    /// <summary>The character being mapped.</summary>
    public char Character { get; }

    /// <summary>The dot/dash pattern the character is keyed as.</summary>
    public string Pattern { get; }

    /// <summary>
    /// Whether the entry is encode-only. When <c>true</c>, decoding <see cref="Pattern"/> yields a different
    /// character rather than <see cref="Character"/>.
    /// </summary>
    public bool IsAlias { get; }

    /// <inheritdoc />
    public override string ToString() => $"{Character} {Pattern}{(IsAlias ? " (alias)" : string.Empty)}";
}
