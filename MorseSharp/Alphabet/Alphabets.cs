namespace MorseSharp.Alphabet;

/// <summary>
/// Registry of the built alphabets. Tables are built once per process on first use and shared by every thread.
/// </summary>
internal static class Alphabets
{
    public static readonly MorseAlphabet English = EnglishCharacters.Build();
    public static readonly MorseAlphabet Kurdish = KurdishCharacters.Build();
    public static readonly MorseAlphabet KurdishLatin = KurdishLatinCharacters.Build();
    public static readonly MorseAlphabet Arabic = ArabicCharacters.Build();
    public static readonly MorseAlphabet Deutsch = DeutschCharacters.Build();
    public static readonly MorseAlphabet Spanish = SpanishCharacters.Build();
    public static readonly MorseAlphabet French = FrenchCharacters.Build();
    public static readonly MorseAlphabet Italian = ItalianCharacters.Build();
    public static readonly MorseAlphabet Japanese = JapaneseCharacters.Build();
    public static readonly MorseAlphabet Portugues = PortuguesCharacters.Build();
    public static readonly MorseAlphabet Russian = RussianCharacters.Build();

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

    /// <summary>Returns the raw data an alphabet was built from (for tests and tooling).</summary>
    public static ((char Char, string Code)[][] Primaries, (char Char, string Code)[] Aliases) DataFor(Language language) => language switch
    {
        Language.English => (EnglishCharacters.Primaries, EnglishCharacters.Aliases),
        Language.Kurdish => (KurdishCharacters.Primaries, KurdishCharacters.Aliases),
        Language.KurdishLatin => (KurdishLatinCharacters.Primaries, KurdishLatinCharacters.Aliases),
        Language.Arabic => (ArabicCharacters.Primaries, ArabicCharacters.Aliases),
        Language.Deutsch => (DeutschCharacters.Primaries, DeutschCharacters.Aliases),
        Language.Spanish => (SpanishCharacters.Primaries, SpanishCharacters.Aliases),
        Language.French => (FrenchCharacters.Primaries, FrenchCharacters.Aliases),
        Language.Italian => (ItalianCharacters.Primaries, ItalianCharacters.Aliases),
        Language.Japanese => (JapaneseCharacters.Primaries, JapaneseCharacters.Aliases),
        Language.Portugues => (PortuguesCharacters.Primaries, PortuguesCharacters.Aliases),
        Language.Russian => (RussianCharacters.Primaries, RussianCharacters.Aliases),
        _ => throw new ArgumentOutOfRangeException(nameof(language), language, "Unsupported language."),
    };
}
