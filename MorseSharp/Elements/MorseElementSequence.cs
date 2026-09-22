namespace MorseSharp;

/// <summary>
/// The elements of one Morse sequence, at a fixed timing. Enumerate it with <c>foreach</c>.
/// </summary>
/// <remarks>
/// A snapshot: it captures the text, alphabet and timing of the chain that produced it, so it survives past the next
/// chain on the same thread and can be enumerated as often as you like. Enumerating allocates nothing.
/// </remarks>
public readonly struct MorseElementSequence
{
    private readonly string _content;
    private readonly MorseAlphabet? _alphabet;
    private readonly MorseTiming _timing;

    internal MorseElementSequence(string content, MorseAlphabet? alphabet, MorseTiming timing)
    {
        _content = content;
        _alphabet = alphabet;
        _timing = timing;
    }

    /// <summary>Returns a fresh enumerator over the sequence.</summary>
    public MorseElementEnumerator GetEnumerator() => new(_content, _alphabet, _timing);

    /// <summary>
    /// How long the whole sequence takes. Walks the elements to add them up, so cache it if you need it repeatedly.
    /// </summary>
    public TimeSpan Duration
    {
        get
        {
            TimeSpan total = TimeSpan.Zero;
            foreach (MorseElement element in this)
                total += element.Duration;

            return total;
        }
    }
}
