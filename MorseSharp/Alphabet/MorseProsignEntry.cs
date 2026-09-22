namespace MorseSharp;

/// <summary>A prosign an alphabet defines, and the pattern it is keyed as.</summary>
/// <remarks>
/// Returned by <see cref="MorseAlphabet.Prosigns"/>. A prosign is two or more letters keyed as one unbroken
/// sequence, written in text as <c>&lt;NAME&gt;</c>.
/// </remarks>
public readonly struct MorseProsignEntry
{
    internal MorseProsignEntry(string name, string pattern, bool isAlias)
    {
        Name = name;
        Pattern = pattern;
        IsAlias = isAlias;
    }

    /// <summary>The letters it is made of, without brackets, such as <c>AR</c>.</summary>
    public string Name { get; }

    /// <summary>The dot/dash pattern the prosign is keyed as.</summary>
    public string Pattern { get; }

    /// <summary>
    /// Whether the prosign is encode-only because a character or another prosign owns <see cref="Pattern"/> for
    /// decoding. <c>&lt;AR&gt;</c> is the same signal as <c>+</c>, which keeps the pattern.
    /// </summary>
    public bool IsAlias { get; }

    /// <inheritdoc />
    public override string ToString() => $"<{Name}> {Pattern}{(IsAlias ? " (alias)" : string.Empty)}";
}
