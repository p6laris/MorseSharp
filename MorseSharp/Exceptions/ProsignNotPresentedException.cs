namespace MorseSharp.Exceptions;

/// <summary>
/// Thrown when text contains a bracketed prosign that the selected alphabet does not define.
/// </summary>
public sealed class ProsignNotPresentedException(string prosign, string alphabetName)
    : Exception($"The prosign '<{prosign}>' is not present in the {alphabetName} alphabet.")
{
    /// <summary>The offending prosign, without its brackets.</summary>
    public string Prosign { get; } = prosign;

    /// <summary>The name of the alphabet that was selected.</summary>
    public string AlphabetName { get; } = alphabetName;
}
