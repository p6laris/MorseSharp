using MorseSharp.Alphabet;

namespace MorseTest;

public class AlphabetInspectionTests
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

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void ForLanguageReturnsTheSameInstanceTheChainUses(Language language)
    {
        Assert.Same(Alphabets.For(language), MorseAlphabet.ForLanguage(language));
    }

    [Fact]
    public void ForLanguageRejectsAnUndefinedValue()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => MorseAlphabet.ForLanguage((Language)200));
    }

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void CharactersListEveryEntryTheAlphabetWasPackedFrom(Language language)
    {
        MorseAlphabet alphabet = MorseAlphabet.ForLanguage(language);

        Assert.Equal(
            alphabet.Entries.Select(entry => (entry.Character, entry.Pattern, entry.IsAlias)),
            alphabet.Characters.Select(entry => (entry.Character, entry.Pattern, entry.IsAlias)));
    }

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void EveryListedPrimaryCharacterEncodesToItsListedPattern(Language language)
    {
        var conv = Morse.GetConverter().ForLanguage(language);

        foreach (MorseCharacterEntry entry in MorseAlphabet.ForLanguage(language).Characters)
        {
            if (!entry.IsAlias)
                Assert.Equal(entry.Pattern, conv.ToMorse(entry.Character.ToString()).Encode());
        }
    }

    [Fact]
    public void EnglishProsignsAreListedWithTheirPatternsAndOwnership()
    {
        IReadOnlyList<MorseProsignEntry> prosigns = MorseAlphabet.ForLanguage(Language.English).Prosigns;

        (string, string, bool)[] expected =
        [
            ("SK", "...-.-", false), ("SN", "...-.", false), ("CT", "-.-.-", false), ("HH", "........", false),
            ("AR", ".-.-.", true), ("BT", "-...-", true), ("KN", "-.--.", true), ("AS", ".-...", true),
        ];

        Assert.Equal(
            expected.Order(),
            prosigns.Select(prosign => (prosign.Name, prosign.Pattern, prosign.IsAlias)).Order());
    }

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void ProsignsThatOwnTheirPatternAreListedBeforeTheAliases(Language language)
    {
        // Decoding walks only the owners, which sit at the front, so the split has to survive the projection.
        Assert.DoesNotContain(
            MorseAlphabet.ForLanguage(language).Prosigns.SkipWhile(prosign => !prosign.IsAlias),
            prosign => !prosign.IsAlias);
    }

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void EveryListedProsignEncodesToItsListedPattern(Language language)
    {
        var conv = Morse.GetConverter().ForLanguage(language);

        foreach (MorseProsignEntry prosign in MorseAlphabet.ForLanguage(language).Prosigns)
            Assert.Equal(prosign.Pattern, conv.ToMorse($"<{prosign.Name}>").Encode());
    }

    [Fact]
    public void ACustomAlphabetListsWhatWasBuiltIntoIt()
    {
        MorseAlphabet klingon = new MorseAlphabetBuilder("Klingon")
            .Add('a', ".-")
            .AddAlias('x', ".-")
            .AddProsign("QQ", "--.--")
            .Build();

        Assert.Equal(
            [('a', ".-", false), ('x', ".-", true)],
            klingon.Characters.Select(entry => (entry.Character, entry.Pattern, entry.IsAlias)));

        MorseProsignEntry prosign = Assert.Single(klingon.Prosigns);
        Assert.Equal(("QQ", "--.--", false), (prosign.Name, prosign.Pattern, prosign.IsAlias));
    }

    [Fact]
    public void AnAlphabetWithoutProsignsListsNone()
    {
        MorseAlphabet klingon = new MorseAlphabetBuilder("Klingon").Add('a', ".-").Build();
        Assert.Empty(klingon.Prosigns);
    }

    [Fact]
    public void ListingsAreBuiltOnceAndReused()
    {
        MorseAlphabet english = MorseAlphabet.ForLanguage(Language.English);

        Assert.Same(english.Characters, english.Characters);
        Assert.Same(english.Prosigns, english.Prosigns);
    }
}
