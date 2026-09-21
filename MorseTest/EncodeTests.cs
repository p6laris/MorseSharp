namespace MorseTest;

public class EncodeTests
{
    private static ICanSetConversionOption English => Morse.GetConverter().ForLanguage(Language.English);

    [Fact]
    public void SingleCharacterHasNoSeparator()
    {
        Assert.Equal(".", English.ToMorse("E").Encode());
        Assert.Equal("/", English.ToMorse(" ").Encode());
    }

    [Fact]
    public void WordsAreSeparatedBySlash()
    {
        Assert.Equal(".... .. / - .... . .-. .", English.ToMorse("HI THERE").Encode());
    }

    [Fact]
    public void EncodeIsRepeatable()
    {
        var chain = English.ToMorse("SOS");
        Assert.Equal("... --- ...", chain.Encode());
        Assert.Equal("... --- ...", chain.Encode());
    }

    [Fact]
    public void LongTextEncodesToTheExactLength()
    {
        string text = string.Concat(Enumerable.Repeat("HELLO WORLD ", 500)).TrimEnd();
        string morse = English.ToMorse(text).Encode();

        Assert.Equal(text.Length - 1 + text.Sum(CodeLength), morse.Length);
        Assert.Equal(text, English.Decode(morse));

        static int CodeLength(char ch) => ch switch
        {
            ' ' => 1,
            'H' => 4, 'E' => 1, 'L' => 4, 'O' => 3,
            'W' => 3, 'R' => 3, 'D' => 3,
            _ => throw new InvalidOperationException($"unexpected '{ch}'"),
        };
    }

    [Fact]
    public void LongestPatternEncodesAndDecodes()
    {
        // The Kurdish full stop is seven symbols, the longest pattern shipped.
        var kurdish = Morse.GetConverter().ForLanguage(Language.Kurdish);
        Assert.Equal(".-.-.-.", kurdish.ToMorse(".").Encode());
        Assert.Equal(".", kurdish.Decode(".-.-.-."));
    }

    [Fact]
    public void AllPunctuationRoundTrips()
    {
        const string punctuation = ".,?;:/'\"_+-=()$&@!";
        var conv = Morse.GetConverter().ForLanguage(Language.English);
        Assert.Equal(punctuation, conv.Decode(conv.ToMorse(punctuation).Encode()));
    }

    [Fact]
    public void DigitsRoundTrip()
    {
        const string digits = "0123456789";
        var conv = Morse.GetConverter().ForLanguage(Language.English);
        Assert.Equal(digits, conv.Decode(conv.ToMorse(digits).Encode()));
    }

    [Fact]
    public void StartingAManualMorseChainDoesNotLeakIntoTheNextEncode()
    {
        // ToAudio(morse) puts the chain in manual-Morse mode; the next ToMorse must reset it.
        Morse.GetConverter().ForLanguage(Language.English).ToAudio("... --- ...");
        Assert.Equal(".", English.ToMorse("E").Encode());
    }
}
