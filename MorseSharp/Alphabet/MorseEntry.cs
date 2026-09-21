using System;

namespace MorseSharp.Alphabet;

/// <summary>
/// One character and the dot/dash pattern it is keyed as, plus whether it owns that pattern for decoding.
/// </summary>
/// <remarks>
/// Shared verbatim with the source generator, so it must stay netstandard2.0-compatible: a plain readonly struct
/// with get-only properties, no record and no <c>init</c> accessor, which would need an IsExternalInit polyfill.
/// </remarks>
internal readonly struct MorseEntry : IEquatable<MorseEntry>
{
    /// <summary>Creates an entry.</summary>
    /// <param name="character">The character being mapped.</param>
    /// <param name="pattern">Its pattern, made only of <c>.</c> and <c>-</c>.</param>
    /// <param name="isAlias">
    /// <c>true</c> when the entry is encode-only. An alias never takes a pattern away from a primary entry, so
    /// decoding that pattern yields the primary character instead.
    /// </param>
    public MorseEntry(char character, string pattern, bool isAlias)
    {
        Character = character;
        Pattern = pattern;
        IsAlias = isAlias;
    }

    /// <summary>The character being mapped.</summary>
    public char Character { get; }

    /// <summary>The dot/dash pattern the character is keyed as.</summary>
    public string Pattern { get; }

    /// <summary>Whether the entry is encode-only.</summary>
    public bool IsAlias { get; }

    /// <inheritdoc />
    public bool Equals(MorseEntry other)
        => Character == other.Character && IsAlias == other.IsAlias && string.Equals(Pattern, other.Pattern, StringComparison.Ordinal);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is MorseEntry other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = Character;
            hash = (hash * 397) ^ (Pattern?.GetHashCode() ?? 0);
            return (hash * 397) ^ (IsAlias ? 1 : 0);
        }
    }

    /// <inheritdoc />
    public override string ToString() => $"{Character} {Pattern}{(IsAlias ? " (alias)" : string.Empty)}";
}
