namespace MorseSharp.Generators;

/// <summary>
/// A problem in a <c>.morse</c> file, reported to the compiler so it appears as a build error rather than as an
/// exception the first time that language is used at run time.
/// </summary>
internal sealed class AlphabetError
{
    /// <summary>Creates an error.</summary>
    /// <param name="line">Zero-based line it was found on, or -1 when it applies to the whole file.</param>
    /// <param name="message">What is wrong.</param>
    public AlphabetError(int line, string message)
    {
        Line = line;
        Message = message;
    }

    /// <summary>Zero-based line, or -1 for a file-level problem.</summary>
    public int Line { get; }

    /// <summary>What is wrong.</summary>
    public string Message { get; }
}
