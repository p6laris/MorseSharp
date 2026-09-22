namespace MorseTest;

public class AlphabetTests
{
    public static TheoryData<Language> AllLanguages
    {
        get
        {
            TheoryData<Language> data = [];
            foreach (Language language in Enum.GetValues<Language>())
                data.Add(language);
            return data;
        }
    }

    public static TheoryData<Language> LatinLanguages =>
    [
        Language.English, Language.Deutsch, Language.Spanish, Language.French,
        Language.Italian, Language.Portugues, Language.KurdishLatin,
    ];

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void EveryPrimaryEntryRoundTrips(Language language)
    {
        var conv = Morse.GetConverter().ForLanguage(language);

        foreach (MorseEntry entry in Alphabets.For(language).Entries)
        {
            if (entry.IsAlias)
                continue;

            Assert.Equal(entry.Pattern, conv.ToMorse(entry.Character.ToString()).Encode());
            Assert.Equal(entry.Character, Assert.Single(conv.Decode(entry.Pattern)));
        }
    }

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void EveryAliasEncodes(Language language)
    {
        var conv = Morse.GetConverter().ForLanguage(language);

        foreach (MorseEntry entry in Alphabets.For(language).Entries)
        {
            if (entry.IsAlias)
                Assert.Equal(entry.Pattern, conv.ToMorse(entry.Character.ToString()).Encode());
        }
    }

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void SpaceRoundTripsAsWordSeparator(Language language)
    {
        var conv = Morse.GetConverter().ForLanguage(language);
        Assert.Equal("/", conv.ToMorse(" ").Encode());
        Assert.Equal(" ", conv.Decode("/"));
    }

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void HashTableProbesStayShort(Language language)
    {
        MorseAlphabet alphabet = Alphabets.For(language);
        Assert.InRange(alphabet.MaxProbeLength, 0, 4);
    }

    [Theory]
    [MemberData(nameof(LatinLanguages))]
    public void ParenthesesFollowItu(Language language)
    {
        var conv = Morse.GetConverter().ForLanguage(language);
        Assert.Equal("-.--.", conv.ToMorse("(").Encode());
        Assert.Equal("-.--.-", conv.ToMorse(")").Encode());
    }

    [Fact]
    public void KurdishSinDecodes()
    {
        // Regression: the hand-written reverse table used to miss this letter.
        Assert.Equal("س", Morse.GetConverter().ForLanguage(Language.Kurdish).Decode("..."));
    }

    [Fact]
    public void GermanLowerCaseSharpSEncodes()
    {
        Assert.Equal("--. .-. --- ......", Morse.GetConverter().ForLanguage(Language.Deutsch).ToMorse("groß").Encode());
    }

    [Theory]
    [InlineData(Language.English, "hello", "HELLO")]
    [InlineData(Language.KurdishLatin, "cem vî fekoyê", "CEM VÎ FEKOYÊ")]
    [InlineData(Language.Russian, "съешь ещё", "СЪЕШЬ ЕЩЁ")]
    [InlineData(Language.Deutsch, "zwölf boxkämpfer", "ZWÖLF BOXKÄMPFER")]
    public void EncodingIgnoresCase(Language language, string lower, string upper)
    {
        var conv = Morse.GetConverter().ForLanguage(language);
        Assert.Equal(conv.ToMorse(upper).Encode(), conv.ToMorse(lower).Encode());
    }

    [Fact]
    public void UnsupportedLanguageThrows()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Morse.GetConverter().ForLanguage((Language)0));
        Assert.Throws<ArgumentOutOfRangeException>(() => Morse.GetConverter().ForLanguage((Language)99));
    }

    [Fact]
    public void TreeCodesRoundTripForEveryPattern()
    {
        // Every dot/dash pattern of 1 to 8 symbols must survive ParseCode -> WriteCode unchanged.
        Span<char> buffer = stackalloc char[MorseAlphabet.MaxSymbols];
        for (int length = 1; length <= MorseAlphabet.MaxSymbols; length++)
        {
            for (int bits = 0; bits < 1 << length; bits++)
            {
                string pattern = string.Create(length, bits, (span, b) =>
                {
                    for (int i = 0; i < span.Length; i++)
                        span[i] = ((b >> (span.Length - 1 - i)) & 1) != 0 ? '-' : '.';
                });

                int code = MorseAlphabet.ParseCode(pattern);
                Assert.Equal(length, MorseAlphabet.SymbolCount(code));

                int position = 0;
                MorseAlphabet.WriteCode(buffer, ref position, code);
                Assert.Equal(pattern, buffer[..position].ToString());
            }
        }
    }

    [Fact]
    public void PatternsLongerThanEightSymbolsAreRejectedAtBuildTime()
    {
        Assert.Throws<ArgumentException>(() => MorseAlphabet.ParseCode("........."));
        Assert.Throws<ArgumentException>(() => MorseAlphabet.ParseCode(""));
        Assert.Throws<ArgumentException>(() => MorseAlphabet.ParseCode(".x."));
    }

    [Fact]
    public void DuplicatePrimaryPatternIsRejectedAtBuildTime()
    {
        MorseAlphabetBuilder builder = new MorseAlphabetBuilder("Duplicate")
            .Add('A', ".-")
            .Add('B', ".-");

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }
}
