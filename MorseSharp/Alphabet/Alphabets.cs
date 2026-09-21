namespace MorseSharp.Alphabet;

/// <summary>
/// Registry of the built-in alphabets. Tables are built once per process on first use and shared by every thread.
/// </summary>
internal static class Alphabets
{
    public static readonly MorseAlphabet English = Build(Language.English, EnglishCharacters.Primaries, EnglishCharacters.Aliases);
    public static readonly MorseAlphabet Kurdish = Build(Language.Kurdish, KurdishCharacters.Primaries, KurdishCharacters.Aliases);
    public static readonly MorseAlphabet KurdishLatin = Build(Language.KurdishLatin, KurdishLatinCharacters.Primaries, KurdishLatinCharacters.Aliases);
    public static readonly MorseAlphabet Arabic = Build(Language.Arabic, ArabicCharacters.Primaries, ArabicCharacters.Aliases);
    public static readonly MorseAlphabet Deutsch = Build(Language.Deutsch, DeutschCharacters.Primaries, DeutschCharacters.Aliases);
    public static readonly MorseAlphabet Spanish = Build(Language.Spanish, SpanishCharacters.Primaries, SpanishCharacters.Aliases);
    public static readonly MorseAlphabet French = Build(Language.French, FrenchCharacters.Primaries, FrenchCharacters.Aliases);
    public static readonly MorseAlphabet Italian = Build(Language.Italian, ItalianCharacters.Primaries, ItalianCharacters.Aliases);
    public static readonly MorseAlphabet Japanese = Build(Language.Japanese, JapaneseCharacters.Primaries, JapaneseCharacters.Aliases);
    public static readonly MorseAlphabet Portugues = Build(Language.Portugues, PortuguesCharacters.Primaries, PortuguesCharacters.Aliases);
    public static readonly MorseAlphabet Russian = Build(Language.Russian, RussianCharacters.Primaries, RussianCharacters.Aliases);

    /// <summary>Returns the alphabet for a language.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for undefined values.</exception>
    public static MorseAlphabet For(Language language) => language switch
    {
        Language.English => English,
        Language.Kurdish => Kurdish,
        Language.KurdishLatin => KurdishLatin,
        Language.Arabic => Arabic,
        Language.Deutsch => Deutsch,
        Language.Spanish => Spanish,
        Language.French => French,
        Language.Italian => Italian,
        Language.Japanese => Japanese,
        Language.Portugues => Portugues,
        Language.Russian => Russian,
        _ => throw new ArgumentOutOfRangeException(nameof(language), language, "Unsupported language."),
    };

    /// <summary>
    /// Builds a built-in alphabet through the same public builder a caller would use, so there is one packing
    /// path rather than a separate internal one that could drift from it.
    /// </summary>
    private static MorseAlphabet Build(Language language, (char Char, string Code)[][] primaries, (char Char, string Code)[] aliases)
    {
        MorseAlphabetBuilder builder = new(language.ToString());

        foreach ((char Char, string Code)[] group in primaries)
        {
            foreach ((char ch, string code) in group)
                builder.Add(ch, code);
        }

        foreach ((char ch, string code) in aliases)
            builder.AddAlias(ch, code);

        return builder.Build();
    }
}
