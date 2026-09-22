namespace MorseSharp;

/// <summary>
/// Makes up amateur radio callsigns to practise on.
/// </summary>
/// <remarks>
/// Callsigns are the hardest thing to copy on the air and the thing you most need to get right, because they are not
/// words and cannot be guessed from context. The shape is a prefix that says which country, a digit, and a suffix of
/// one to three letters. The prefixes here are real allocations, so what comes out looks like traffic rather than
/// random letters, but any particular callsign may well belong to nobody.
/// </remarks>
public static class Callsign
{
    /// <summary>The longest callsign this produces, for sizing a buffer.</summary>
    public const int MaxLength = 6;

    private static readonly string[] Prefixes =
    [
        "K", "N", "W", "G", "M", "F", "I", "R", "S", "T",
        "DL", "EA", "JA", "VK", "ZL", "PY", "LU", "SP", "OK", "OH",
        "SM", "LA", "YO", "YI", "VE", "VU", "HB", "ON", "PA", "LZ",
        "4X", "9A", "3A", "5B", "7X",
    ];

    /// <summary>Returns a callsign.</summary>
    /// <param name="random">Source of randomness; pass a seeded one to get the same callsign every time.</param>
    public static string Next(Random? random = null)
    {
        Span<char> buffer = stackalloc char[MaxLength];
        return new string(buffer[..Next(buffer, random)]);
    }

    /// <summary>Writes a callsign into <paramref name="destination"/> and returns how many characters it wrote.</summary>
    /// <param name="destination">Buffer of at least <see cref="MaxLength"/> characters.</param>
    /// <param name="random">Source of randomness; pass a seeded one to get the same callsign every time.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="destination"/> is shorter than <see cref="MaxLength"/>.</exception>
    public static int Next(Span<char> destination, Random? random = null)
    {
        if (destination.Length < MaxLength)
            throw new ArgumentException($"A callsign needs up to {MaxLength} characters, but only {destination.Length} were given.", nameof(destination));

        Random source = random ?? Random.Shared;

        string prefix = Prefixes[source.Next(Prefixes.Length)];
        prefix.AsSpan().CopyTo(destination);
        int written = prefix.Length;

        destination[written++] = (char)('0' + source.Next(10));

        // One-letter suffixes are rare and mostly historic, so weight towards two and three.
        int suffix = source.Next(10) == 0 ? 1 : source.Next(2, 4);
        for (int i = 0; i < suffix; i++)
            destination[written++] = (char)('A' + source.Next(26));

        return written;
    }
}
