namespace MorseSharp;

/// <summary>
/// Turns keyed elements and the pauses between them back into text.
/// </summary>
/// <remarks>
/// <para>
/// A keyer knows nothing about characters; it only knows dots, dashes and the gaps inside a character. What separates
/// one character from the next is the operator pausing, so this needs to be told about the silence as well as the
/// elements. Feed it what <see cref="IambicKeyer.TryRead"/> returns, and report how long it has been quiet whenever
/// nothing is being sent.
/// </para>
/// <para>
/// Gaps are judged against the halfway points rather than the exact lengths, so hand keying that runs long or short
/// still reads correctly: anything past two dits ends a character, and anything past five ends a word.
/// </para>
/// </remarks>
public sealed class KeyerDecoder
{
    private readonly MorseAlphabet _alphabet;
    private readonly TimeSpan _characterPause;
    private readonly TimeSpan _wordPause;

    private MorseSymbolAccumulator _accumulator = new();
    private TimeSpan _quiet;
    private bool _characterEmitted = true;
    private bool _wordEmitted = true;

    /// <summary>Creates a decoder for one of the built-in languages.</summary>
    /// <param name="language">Alphabet to resolve sequences against.</param>
    /// <param name="wordsPerMinute">The speed the operator is keying at, used to size the pauses.</param>
    /// <exception cref="NotSupportedException">Thrown when <paramref name="language"/> is not a defined language.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="wordsPerMinute"/> is not positive.</exception>
    public KeyerDecoder(Language language = Language.English, int wordsPerMinute = 25)
        : this(Alphabets.For(language), wordsPerMinute)
    {
    }

    /// <summary>Creates a decoder for a custom alphabet.</summary>
    /// <param name="alphabet">Alphabet to resolve sequences against.</param>
    /// <param name="wordsPerMinute">The speed the operator is keying at, used to size the pauses.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="alphabet"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="wordsPerMinute"/> is not positive.</exception>
    public KeyerDecoder(MorseAlphabet alphabet, int wordsPerMinute = 25)
    {
        ArgumentNullException.ThrowIfNull(alphabet);

        MorseTiming timing = new(wordsPerMinute, wordsPerMinute);
        TimeSpan dit = timing.DurationOf(MorseElementKind.Dot);

        _alphabet = alphabet;
        _characterPause = dit * 2;   // between the one dit inside a character and the three between characters
        _wordPause = dit * 5;        // between those three and the seven between words
    }

    /// <summary>Adds an element that has just been keyed. Gaps are ignored; report silence with <see cref="Quiet"/>.</summary>
    /// <param name="element">An element from <see cref="IambicKeyer.TryRead"/>.</param>
    public void Add(MorseElement element)
    {
        if (element.Kind is not (MorseElementKind.Dot or MorseElementKind.Dash))
            return;

        _accumulator.Add(element.Kind == MorseElementKind.Dash);
        _quiet = TimeSpan.Zero;
        _characterEmitted = false;
        _wordEmitted = false;
    }

    /// <summary>
    /// Reports how long it has been since the last element was keyed. Call it as often as you like; the same pause
    /// reported repeatedly produces a character once, not once per call.
    /// </summary>
    /// <param name="sinceLastElement">Time elapsed since the end of the last dot or dash.</param>
    public void Quiet(TimeSpan sinceLastElement) => _quiet = sinceLastElement;

    /// <summary>
    /// Takes the next decoded token, if the pause so far has produced one.
    /// </summary>
    /// <param name="character">
    /// The decoded character, a space when a word has ended, or <c>?</c> when the alphabet defines nothing for the
    /// sequence. Not meaningful when <paramref name="prosign"/> is set.
    /// </param>
    /// <param name="prosign">
    /// The procedural signal in brackets, such as <c>&lt;SK&gt;</c>, when one owns the pattern, otherwise
    /// <c>null</c>. Bracketed so it reads the same as what <c>Decode</c> and <c>FromAudio</c> produce.
    /// </param>
    public bool TryRead(out char character, out string? prosign)
    {
        if (!_characterEmitted && _quiet >= _characterPause && _accumulator.HasSymbols)
        {
            _characterEmitted = true;
            return Resolve(out character, out prosign);
        }

        if (!_wordEmitted && _characterEmitted && _quiet >= _wordPause)
        {
            _wordEmitted = true;
            character = ' ';
            prosign = null;
            return true;
        }

        character = '\0';
        prosign = null;
        return false;
    }

    /// <summary>Resolves anything still collected, as if the operator had stopped for good.</summary>
    /// <inheritdoc cref="TryRead" path="/param"/>
    public bool Flush(out char character, out string? prosign)
    {
        _characterEmitted = true;
        return Resolve(out character, out prosign);
    }

    private bool Resolve(out char character, out string? prosign)
    {
        if (!_accumulator.TryResolve(_alphabet, out character, out string? name))
        {
            prosign = null;
            return false;
        }

        prosign = name is null
            ? null
            : string.Concat(MorseTextScanner.ProsignStart.ToString(), name, MorseTextScanner.ProsignEnd.ToString());

        return true;
    }
}
