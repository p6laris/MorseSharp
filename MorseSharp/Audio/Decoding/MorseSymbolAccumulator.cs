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
    /// <summary>Stands in for a sequence the alphabet does not define.</summary>
    public const char Unknown = '?';

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
    /// <param name="character">
    /// The decoded character, or <see cref="Unknown"/> when the alphabet defines nothing for the sequence. Not
    /// meaningful when <paramref name="prosign"/> is set.
    /// </param>
    /// <param name="prosign">
    /// The prosign letters when a procedural signal owns the pattern, otherwise <c>null</c>. Reported separately
    /// because a prosign is several characters and cannot be returned as one.
    /// </param>
    /// <returns><c>false</c> when nothing had been collected, in which case there is nothing to emit.</returns>
    public bool TryResolve(MorseAlphabet alphabet, out char character, out string? prosign)
    {
        if (!HasSymbols)
        {
            character = '\0';
            prosign = null;
            return false;
        }

        int code = _code;
        _code = MorseAlphabet.WordSpaceCode;

        char decoded = alphabet.Decode(code);
        if (decoded != '\0')
        {
            character = decoded;
            prosign = null;
            return true;
        }

        if (alphabet.TryGetProsignName(code, out string name))
        {
            character = '\0';
            prosign = name;
            return true;
        }

        character = Unknown;
        prosign = null;
        return true;
    }

    /// <summary>Discards any collected symbols.</summary>
    public void Clear() => _code = MorseAlphabet.WordSpaceCode;
}
