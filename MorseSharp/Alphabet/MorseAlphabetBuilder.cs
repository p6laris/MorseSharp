namespace MorseSharp;

/// <summary>
/// Builds a <see cref="MorseAlphabet"/>, either from scratch or by extending an existing one.
/// </summary>
/// <remarks>
/// <para>
/// The builder collects entries and packs them once, when <see cref="Build"/> is called. Nothing is written to a
/// lookup table until then, which is why <see cref="Remove"/> can simply drop an entry: the tables are always built
/// from the surviving entries rather than edited in place.
/// </para>
/// <para>
/// A builder may be reused. <see cref="Build"/> copies the entries it packs, so alphabets built earlier are
/// unaffected by later changes.
/// </para>
/// <example>
/// <code>
/// var klingon = new MorseAlphabetBuilder("Klingon")
///     .Add('a', ".-")
///     .Add('b', "-...")
///     .Build();
///
/// var extended = MorseAlphabetBuilder.From(Language.Deutsch)
///     .Add('Ə', "..--.")
///     .Remove('$')
///     .Build();
/// </code>
/// </example>
/// </remarks>
public sealed class MorseAlphabetBuilder
{
    private readonly List<MorseEntry> _entries = [];

    /// <summary>The name the built alphabet will carry, used in error messages.</summary>
    public string Name { get; }

    /// <summary>Starts an empty alphabet.</summary>
    /// <param name="name">A name for the alphabet, for example <c>"Klingon"</c>.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null, empty or whitespace.</exception>
    public MorseAlphabetBuilder(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
    }

    /// <summary>Starts from one of the built-in languages, so it can be extended or trimmed.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="language"/> is not a defined value.</exception>
    public static MorseAlphabetBuilder From(Language language) => From(Alphabets.For(language));

    /// <summary>Starts from an existing alphabet, including one built earlier by this API.</summary>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="alphabet"/> is <c>null</c>.</exception>
    public static MorseAlphabetBuilder From(MorseAlphabet alphabet)
    {
        ArgumentNullException.ThrowIfNull(alphabet);

        MorseAlphabetBuilder builder = new(alphabet.Name);
        builder._entries.AddRange(alphabet.Entries);
        return builder;
    }

    /// <summary>
    /// Adds a character and the pattern it is keyed as. The character owns that pattern for decoding.
    /// </summary>
    /// <param name="character">The character. Its upper and lower case forms are both accepted when encoding.</param>
    /// <param name="pattern">Dots and dashes, at most <see cref="MorseAlphabet.MaxSymbols"/> of them.</param>
    /// <exception cref="ArgumentException">Thrown when the pattern is empty, too long, or contains another symbol.</exception>
    public MorseAlphabetBuilder Add(char character, string pattern) => AddEntry(character, pattern, isAlias: false);

    /// <summary>
    /// Adds an encode-only character. It can be encoded, but the pattern keeps decoding to whichever character was
    /// added with <see cref="Add"/>, which is how two characters can share one pattern.
    /// </summary>
    /// <param name="character">The character. Its upper and lower case forms are both accepted when encoding.</param>
    /// <param name="pattern">Dots and dashes, at most <see cref="MorseAlphabet.MaxSymbols"/> of them.</param>
    /// <exception cref="ArgumentException">Thrown when the pattern is empty, too long, or contains another symbol.</exception>
    public MorseAlphabetBuilder AddAlias(char character, string pattern) => AddEntry(character, pattern, isAlias: true);

    /// <summary>
    /// Drops every entry for a character. If it owned a pattern that an alias also uses, that alias takes the
    /// pattern over when the alphabet is built.
    /// </summary>
    /// <param name="character">The character to remove. Removing one that is not present does nothing.</param>
    public MorseAlphabetBuilder Remove(char character)
    {
        _entries.RemoveAll(entry => entry.Character == character);
        return this;
    }

    /// <summary>Packs the entries collected so far into an alphabet.</summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the alphabet is empty, when two characters added with <see cref="Add"/> share a pattern, or when
    /// one character was given two different patterns.
    /// </exception>
    public MorseAlphabet Build()
    {
        if (_entries.Count == 0)
            throw new InvalidOperationException($"{Name}: an alphabet needs at least one character.");

        return MorseAlphabet.Pack(Name, _entries);
    }

    private MorseAlphabetBuilder AddEntry(char character, string pattern, bool isAlias)
    {
        ArgumentException.ThrowIfNullOrEmpty(pattern);

        // Validate here rather than at Build, so a bad pattern is reported at the call that introduced it.
        MorseAlphabet.ParseCode(pattern);

        _entries.Add(new MorseEntry(character, pattern, isAlias));
        return this;
    }
}
