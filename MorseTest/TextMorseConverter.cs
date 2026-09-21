namespace MorseTest;

public class TextMorseConverterTest
{
    [Fact]
    public void ConvertToMorseWithValidString()
    {
        var morse = Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse("The quick brown fox jumps over the lazy dog")
            .Encode();

        Assert.True(morse.Length > 0);
    }

    [Fact]
    public void ConvertToMorseWithNullString()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToMorse(null!)
                .Encode());
    }

    [Fact]
    public void ConvertToMorseWithEmptyString()
    {
        Assert.Throws<ArgumentException>(() =>
            Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToMorse("")
                .Encode());
    }

    [Fact]
    public void ConvertToMorseWithInvalidCharacter()
    {
        var ex = Assert.Throws<CharacterNotPresentedException>(() =>
            Morse.GetConverter()
                .ForLanguage(Language.Kurdish)
                .ToMorse("~")
                .Encode());

        Assert.Equal('~', ex.Character);
        Assert.Equal(Language.Kurdish, ex.Language);
    }

    [Fact]
    public void ConvertToTextWithValidMorse()
    {
        var text = Morse.GetConverter()
            .ForLanguage(Language.English)
            .Decode(".... ..");

        Assert.Equal("HI", text);
    }

    [Fact]
    public void ConvertToTextWithNullMorse()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Morse.GetConverter()
                .ForLanguage(Language.Kurdish)
                .Decode(null!));
    }

    [Fact]
    public void ConvertTextWithInvalidMorse()
    {
        Assert.Throws<SequenceNotFoundException>(() =>
            Morse.GetConverter()
                .ForLanguage(Language.Kurdish)
                .Decode("............"));
    }

    [Fact]
    public void ConvertToTextWithMoreThanOneWords()
    {
        var text = Morse.GetConverter()
            .ForLanguage(Language.English)
            .Decode(".... .. / .... ..");

        Assert.Equal("HI HI", text);
    }

    [Fact]
    public void EncodeThenDecodeRoundTripsAPangram()
    {
        const string original = "THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG 0123456789";
        var conv = Morse.GetConverter().ForLanguage(Language.English);

        var morse = conv.ToMorse(original).Encode();
        Assert.Equal(original, conv.Decode(morse));
    }

    [Fact]
    public void EncodeIsLazyButValidatesEagerly()
    {
        // The exception surfaces from ToMorse, before Encode is called.
        var conv = Morse.GetConverter().ForLanguage(Language.English);
        Assert.Throws<CharacterNotPresentedException>(() => conv.ToMorse("Hi ~"));
    }

    [Fact]
    public void EncodePreservesConsecutiveSpaces()
    {
        var conv = Morse.GetConverter().ForLanguage(Language.English);
        var morse = conv.ToMorse("A  B").Encode();
        Assert.Equal(".- / / -...", morse);
        Assert.Equal("A  B", conv.Decode(morse));
    }
}
