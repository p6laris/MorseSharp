using System;

namespace MorseSharp.Alphabet;

/// <summary>
/// A procedural signal: two or more letters keyed as one unbroken sequence, with no gap between them.
/// </summary>
/// <remarks>
/// <para>
/// Prosigns are instructions rather than text. <c>AR</c> ends a message, <c>BT</c> breaks between sections, <c>KN</c>
/// invites one named station to reply. Because the letters are run together they do not decode as letters, which is
/// what makes them a separate concept from an ordinary character.
/// </para>
/// <para>
/// Several share a pattern with punctuation, since historically they are the same on-air signal: <c>AR</c> is the
/// pattern of <c>+</c>, <c>BT</c> of <c>=</c>. Those are declared as aliases, so they encode while the punctuation
/// keeps ownership of the pattern for decoding.
/// </para>
/// <para>
/// Shared verbatim with the source generator, so it must stay netstandard2.0-compatible. Callers listing an
/// alphabet's prosigns get <see cref="MorseProsignEntry"/> instead, which is not tied to the generator's shape.
/// </para>
/// </remarks>
internal readonly struct MorseProsign
{
    /// <summary>Creates a prosign.</summary>
    /// <param name="name">The letters it is made of, such as <c>AR</c>, written without brackets.</param>
    /// <param name="pattern">Its pattern, made only of <c>.</c> and <c>-</c>.</param>
    /// <param name="isAlias"><c>true</c> when it shares a pattern already owned by a character or another prosign.</param>
    public MorseProsign(string name, string pattern, bool isAlias)
    {
        Name = name;
        Pattern = pattern;
        IsAlias = isAlias;
    }

    /// <summary>The letters it is made of, without brackets.</summary>
    public string Name { get; }

    /// <summary>The dot/dash pattern.</summary>
    public string Pattern { get; }

    /// <summary>Whether it is encode-only because something else owns the pattern.</summary>
    public bool IsAlias { get; }

    /// <inheritdoc />
    public override string ToString() => "<" + Name + "> " + Pattern;
}
