namespace MorseSharp.Audio.Decoding;

/// <summary>
/// Collects the dots and dashes of one character and resolves them when it ends.
/// </summary>
/// <remarks>
/// Symbols are folded into the same tree code the encoder uses, so resolving a finished character is a single array
/// index rather than a search. Both the buffered and the streaming decoder drive this, which keeps the definition of
/// how symbols become characters in one place.
/// </remarks>
internal struct MorseSymbolAccumulator
{
    private int _code;

    /// <summary>Creates an accumulator with no symbols collected.</summary>
    public MorseSymbolAccumulator() => _code = MorseAlphabet.WordSpaceCode;

    /// <summary>Whether any symbol has been collected since the last character was resolved.</summary>
    public readonly bool HasSymbols => _code != MorseAlphabet.WordSpaceCode;

    /// <summary>Adds one symbol.</summary>
    /// <param name="isDash"><c>true</c> for a dash, <c>false</c> for a dot.</param>
    public void Add(bool isDash)
    {
        // Sequences longer than the table can hold stop growing; the lookup then fails cleanly rather than wrapping.
        if (_code < MorseAlphabet.CodeLimit)
            _code = isDash ? (_code << 1) | 1 : _code << 1;
    }

    /// <summary>
    /// Resolves the collected symbols and starts a new character.
    /// </summary>
    /// <param name="alphabet">Alphabet to resolve against.</param>
    /// <param name="character">The decoded character, or <c>?</c> when the sequence is not in the alphabet.</param>
    /// <returns><c>false</c> when nothing had been collected, in which case there is no character to emit.</returns>
    public bool TryResolve(MorseAlphabet alphabet, out char character)
    {
        if (!HasSymbols)
        {
            character = '\0';
            return false;
        }

        char decoded = alphabet.Decode(_code);
        character = decoded == '\0' ? '?' : decoded;
        _code = MorseAlphabet.WordSpaceCode;
        return true;
    }

    /// <summary>Discards any collected symbols.</summary>
    public void Clear() => _code = MorseAlphabet.WordSpaceCode;
}
