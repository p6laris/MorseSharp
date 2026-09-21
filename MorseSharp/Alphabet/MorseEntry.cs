namespace MorseSharp.Alphabet;

/// <summary>
/// One character and the dot/dash pattern it is keyed as, plus whether it owns that pattern for decoding.
/// </summary>
/// <param name="Character">The character being mapped.</param>
/// <param name="Pattern">Its pattern, made only of <c>.</c> and <c>-</c>.</param>
/// <param name="IsAlias">
/// <c>true</c> when the entry is encode-only. An alias never takes a pattern away from a primary entry, so
/// decoding that pattern yields the primary character instead.
/// </param>
internal readonly record struct MorseEntry(char Character, string Pattern, bool IsAlias);
