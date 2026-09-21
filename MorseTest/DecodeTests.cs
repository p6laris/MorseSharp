namespace MorseTest;

public class DecodeTests
{
    private static ICanSetConversionOption English => Morse.GetConverter().ForLanguage(Language.English);

    [Fact]
    public void IgnoresExtraWhitespaceAndNewlines()
    {
        Assert.Equal("HI HI", English.Decode("  ....   ..\t/\r\n.... ..  "));
    }

    [Fact]
    public void SlashWithoutSurroundingSpacesSeparatesWords()
    {
        Assert.Equal("H S", English.Decode("..../..."));
    }

    [Fact]
    public void MultipleSlashesKeepMultipleSpaces()
    {
        Assert.Equal("E  E", English.Decode(". / / ."));
    }

    [Fact]
    public void UnknownSequenceIsReported()
    {
        var ex = Assert.Throws<SequenceNotFoundException>(() => English.Decode(".... ......... .."));
        Assert.Equal(".........", ex.Sequence);
        Assert.Equal(Language.English, ex.Language);
    }

    [Fact]
    public void ForeignSymbolInsideSequenceIsReported()
    {
        var ex = Assert.Throws<SequenceNotFoundException>(() => English.Decode(".... .x. .."));
        Assert.Equal(".x.", ex.Sequence);
    }

    [Fact]
    public void VeryLongSequenceDoesNotOverflow()
    {
        var ex = Assert.Throws<SequenceNotFoundException>(() => English.Decode(new string('.', 100)));
        Assert.Equal(100, ex.Sequence.Length);
    }

    [Fact]
    public void LongInputUsesPooledBuffer()
    {
        string morse = string.Join(' ', Enumerable.Repeat(".", 1000));
        Assert.Equal(new string('E', 1000), English.Decode(morse));
    }

    [Fact]
    public void WhitespaceOnlyDecodesToEmpty()
    {
        Assert.Equal("", English.Decode("   "));
    }

    [Fact]
    public void EmptyThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => English.Decode(""));
    }

    [Fact]
    public void JapaneseDecodesToKanaNotToLatinAlias()
    {
        // ".-.-.-" is the comma 、 in Wabun; the Latin full stop is an encode-only alias.
        var japanese = Morse.GetConverter().ForLanguage(Language.Japanese);
        Assert.Equal("、", japanese.Decode(".-.-.-"));
        Assert.Equal(".-.-.-", japanese.ToMorse(".").Encode());
    }
}
