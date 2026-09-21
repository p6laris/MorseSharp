namespace MorseTest;

public class ProsignTests
{
    private static ICanSetConversionOption English => Morse.GetConverter().ForLanguage(Language.English);

    [Theory]
    [InlineData("SK", "...-.-")]   // end of contact
    [InlineData("SN", "...-.")]    // understood
    [InlineData("CT", "-.-.-")]    // attention, starting
    [InlineData("HH", "........")] // correction
    public void OwnedProsignsRoundTrip(string name, string pattern)
    {
        Assert.Equal(pattern, English.ToMorse($"<{name}>").Encode());
        Assert.Equal($"<{name}>", English.Decode(pattern));
    }

    [Theory]
    [InlineData("AR", ".-.-.", '+')]
    [InlineData("BT", "-...-", '=')]
    [InlineData("KN", "-.--.", '(')]
    [InlineData("AS", ".-...", '&')]
    public void SharedProsignsEncodeButThePunctuationKeepsThePattern(string name, string pattern, char punctuation)
    {
        // These are the same on-air signal, so both spellings encode identically.
        Assert.Equal(pattern, English.ToMorse($"<{name}>").Encode());
        Assert.Equal(pattern, English.ToMorse(punctuation.ToString()).Encode());

        // Decoding has to pick one, and the punctuation owns it, which keeps existing behaviour unchanged.
        Assert.Equal(punctuation.ToString(), English.Decode(pattern));
    }

    [Fact]
    public void ProsignsSitInsideOrdinaryText()
    {
        Assert.Equal(".... .. ...-.-", English.ToMorse("HI<SK>").Encode());
        Assert.Equal("HI<SK>", English.Decode(".... .. ...-.-"));
    }

    [Fact]
    public void ProsignsWorkAcrossWords()
    {
        const string text = "CQ CQ <AR>";
        Assert.Equal("-.-. --.- / -.-. --.- / .-.-.", English.ToMorse(text).Encode());
    }

    [Fact]
    public void AProsignIsKeyedAsOneUnbrokenSignal()
    {
        // <SK> must key as six symbols with no character gap, exactly like the six-symbol pattern written out.
        byte[] fromProsign = English.ToMorse("<SK>").ToAudio().SetAudioOptions(20, 20, 700).GetBytes();
        byte[] fromPattern = Morse.GetConverter().ForLanguage(Language.English)
            .ToAudio("...-.-").SetAudioOptions(20, 20, 700).GetBytes();

        Assert.Equal(fromPattern, fromProsign);
    }

    [Fact]
    public void AProsignIsNotItsLettersSentSeparately()
    {
        // The whole point: run together they are one signal, apart they are two letters.
        Assert.NotEqual(English.ToMorse("SK").Encode(), English.ToMorse("<SK>").Encode());
        Assert.Equal("... -.-", English.ToMorse("SK").Encode());
    }

    [Fact]
    public void ProsignNamesAreCaseInsensitive()
    {
        Assert.Equal("...-.-", English.ToMorse("<sk>").Encode());
    }

    [Fact]
    public void AnUnknownProsignIsReported()
    {
        var ex = Assert.Throws<ProsignNotPresentedException>(() => English.ToMorse("<ZZZ>"));
        Assert.Equal("ZZZ", ex.Prosign);
        Assert.Equal("English", ex.AlphabetName);
    }

    [Fact]
    public void AStrayAngleBracketIsReportedAsACharacter()
    {
        // '<' is not in the alphabet, so an unclosed bracket is an ordinary unknown character rather than a prosign.
        var ex = Assert.Throws<CharacterNotPresentedException>(() => English.ToMorse("A<B"));
        Assert.Equal('<', ex.Character);
    }

    [Fact]
    public void DecodedAudioRecoversAProsign()
    {
        byte[] wav = English.ToMorse("SOS <SK>").ToAudio().SetAudioOptions(20, 20, 700).GetBytes();
        short[] pcm = MemoryMarshal.Cast<byte, short>(wav.AsSpan(44)).ToArray();

        Assert.Equal("SOS <SK>", Morse.GetConverter().ForLanguage(Language.English).FromAudio(pcm, 11025, 700, 20));
    }

    [Fact]
    public void CustomAlphabetsCanDefineProsigns()
    {
        MorseAlphabet alphabet = new MorseAlphabetBuilder("Custom")
            .Add('a', ".-")
            .AddProsign("XY", "-.-.-.-")
            .Build();

        var conv = Morse.GetConverter().ForAlphabet(alphabet);
        Assert.Equal("-.-.-.-", conv.ToMorse("<XY>").Encode());
        Assert.Equal("<XY>", conv.Decode("-.-.-.-"));
    }

    [Fact]
    public void ExtendingALanguageKeepsItsProsigns()
    {
        MorseAlphabet extended = MorseAlphabetBuilder.From(Language.English)
            .Add('Ə', "..--.")
            .Build();

        Assert.Equal("...-.-", Morse.GetConverter().ForAlphabet(extended).ToMorse("<SK>").Encode());
    }

    [Fact]
    public void AProsignCannotClaimAPatternACharacterOwns()
    {
        MorseAlphabetBuilder builder = new MorseAlphabetBuilder("Clash")
            .Add('A', ".-")
            .AddProsign("XY", ".-");

        var ex = Assert.Throws<InvalidOperationException>(() => builder.Build());
        Assert.Contains("alias", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ProsignNamesNeedAtLeastTwoLetters()
    {
        // A single letter is a character, not a procedural signal.
        MorseAlphabetBuilder builder = new("Short");
        Assert.Throws<ArgumentException>(() => builder.AddProsign("", "...-.-"));
    }

    [Fact]
    public void LanguagesWithoutProsignsAreUnaffected()
    {
        var kurdish = Morse.GetConverter().ForLanguage(Language.Kurdish);
        Assert.Throws<ProsignNotPresentedException>(() => kurdish.ToMorse("<SK>"));
    }
}
