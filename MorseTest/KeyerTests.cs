namespace MorseTest;

public class KeyerTests
{
    private const int Wpm = 20;

    private static List<MorseElementKind> Drain(IambicKeyer keyer, int limit = 32)
    {
        List<MorseElementKind> kinds = [];
        while (kinds.Count < limit && keyer.TryRead(out MorseElement element))
            kinds.Add(element.Kind);

        return kinds;
    }

    /// <summary>Keys a pattern one symbol at a time, the way a single-lever paddle is worked.</summary>
    private static List<MorseElement> Key(IambicKeyer keyer, string pattern)
    {
        List<MorseElement> elements = [];
        foreach (char symbol in pattern)
        {
            keyer.Paddles(dot: symbol == '.', dash: symbol == '-');
            Assert.True(keyer.TryRead(out MorseElement keyed));
            elements.Add(keyed);

            keyer.Paddles(dot: false, dash: false);
            Assert.True(keyer.TryRead(out MorseElement gap));
            elements.Add(gap);
        }

        return elements;
    }

    [Fact]
    public void AnUntouchedKeyerSendsNothing()
    {
        IambicKeyer keyer = new(Wpm);

        Assert.True(keyer.IsIdle);
        Assert.False(keyer.TryRead(out _));
    }

    [Fact]
    public void HoldingTheDotPaddleSendsDots()
    {
        IambicKeyer keyer = new(Wpm);
        keyer.Paddles(dot: true, dash: false);

        Assert.Equal(
            [MorseElementKind.Dot, MorseElementKind.ElementGap, MorseElementKind.Dot, MorseElementKind.ElementGap],
            Drain(keyer, 4));
    }

    [Fact]
    public void HoldingTheDashPaddleSendsDashes()
    {
        IambicKeyer keyer = new(Wpm);
        keyer.Paddles(dot: false, dash: true);

        Assert.Equal(
            [MorseElementKind.Dash, MorseElementKind.ElementGap, MorseElementKind.Dash, MorseElementKind.ElementGap],
            Drain(keyer, 4));
    }

    [Fact]
    public void SqueezingBothAlternatesStartingOnADot()
    {
        IambicKeyer keyer = new(Wpm);
        keyer.Paddles(dot: true, dash: true);

        Assert.Equal(
            [
                MorseElementKind.Dot, MorseElementKind.ElementGap,
                MorseElementKind.Dash, MorseElementKind.ElementGap,
                MorseElementKind.Dot, MorseElementKind.ElementGap,
            ],
            Drain(keyer, 6));
    }

    [Fact]
    public void SqueezingAfterADashCarriesOnAlternating()
    {
        IambicKeyer keyer = new(Wpm);

        keyer.Paddles(dot: false, dash: true);
        Assert.True(keyer.TryRead(out MorseElement first));
        Assert.Equal(MorseElementKind.Dash, first.Kind);
        Assert.True(keyer.TryRead(out _));      // the gap

        keyer.Paddles(dot: true, dash: true);
        Assert.True(keyer.TryRead(out MorseElement second));
        Assert.Equal(MorseElementKind.Dot, second.Kind);
    }

    [Fact]
    public void LettingGoStopsIt()
    {
        IambicKeyer keyer = new(Wpm);

        keyer.Paddles(dot: true, dash: false);
        Assert.True(keyer.TryRead(out _));          // the dot
        keyer.Paddles(dot: false, dash: false);
        Assert.True(keyer.TryRead(out _));          // the gap that follows it

        Assert.False(keyer.TryRead(out _));
        Assert.True(keyer.IsIdle);
    }

    [Fact]
    public void ReleasingOneOwnPaddleDoesNotAddAnElement()
    {
        // Mode B adds an element when a squeeze is let go, not when a single paddle is.
        foreach (KeyerMode mode in new[] { KeyerMode.A, KeyerMode.B })
        {
            IambicKeyer keyer = new(Wpm, mode);

            keyer.Paddles(dot: true, dash: false);
            Assert.True(keyer.TryRead(out _));      // the dot
            keyer.Paddles(dot: true, dash: false);  // still held while it plays
            keyer.Paddles(dot: false, dash: false); // let go before it ends
            Assert.True(keyer.TryRead(out _));      // the gap

            Assert.False(keyer.TryRead(out _));
        }
    }

    [Fact]
    public void ModeAStopsWhenTheSqueezeIsReleased()
    {
        IambicKeyer keyer = new(Wpm, KeyerMode.A);

        keyer.Paddles(dot: true, dash: true);
        Assert.True(keyer.TryRead(out MorseElement dot));
        Assert.Equal(MorseElementKind.Dot, dot.Kind);

        keyer.Paddles(dot: true, dash: true);       // still squeezed while the dot plays
        keyer.Paddles(dot: false, dash: false);     // and let go before it ends

        Assert.True(keyer.TryRead(out _));          // the gap after the dot
        Assert.False(keyer.TryRead(out _));         // mode A drops the remembered dash
    }

    [Fact]
    public void ModeBAddsExactlyOneMoreElement()
    {
        IambicKeyer keyer = new(Wpm, KeyerMode.B);

        keyer.Paddles(dot: true, dash: true);
        Assert.True(keyer.TryRead(out MorseElement dot));
        Assert.Equal(MorseElementKind.Dot, dot.Kind);

        keyer.Paddles(dot: true, dash: true);
        keyer.Paddles(dot: false, dash: false);

        Assert.True(keyer.TryRead(out _));          // the gap after the dot
        Assert.True(keyer.TryRead(out MorseElement extra));
        Assert.Equal(MorseElementKind.Dash, extra.Kind);

        Assert.True(keyer.TryRead(out _));          // the gap after that
        Assert.False(keyer.TryRead(out _));         // and no more than one
    }

    [Fact]
    public void ATapDuringAnElementIsNotLost()
    {
        IambicKeyer keyer = new(Wpm);

        keyer.Paddles(dot: false, dash: true);
        Assert.True(keyer.TryRead(out MorseElement dash));
        Assert.Equal(MorseElementKind.Dash, dash.Kind);

        // A dot tapped and released while the dash is still playing.
        keyer.Paddles(dot: true, dash: true);
        keyer.Paddles(dot: false, dash: true);

        Assert.True(keyer.TryRead(out _));          // the gap
        Assert.True(keyer.TryRead(out MorseElement next));
        Assert.Equal(MorseElementKind.Dot, next.Kind);
    }

    [Fact]
    public void ElementsCarryTheKeyingSpeed()
    {
        IambicKeyer keyer = new(Wpm);
        keyer.Paddles(dot: true, dash: true);

        Assert.True(keyer.TryRead(out MorseElement dot));
        Assert.True(keyer.TryRead(out MorseElement gap));
        Assert.True(keyer.TryRead(out MorseElement dash));

        Assert.Equal(TimeSpan.FromMilliseconds(60), dot.Duration);
        Assert.Equal(TimeSpan.FromMilliseconds(60), gap.Duration);
        Assert.Equal(TimeSpan.FromMilliseconds(180), dash.Duration);
        Assert.True(dot.KeyDown);
        Assert.False(gap.KeyDown);
    }

    [Fact]
    public void KeyedElementsMatchWhatTheEncoderWouldSend()
    {
        IambicKeyer keyer = new(Wpm);
        List<MorseElement> keyed = Key(keyer, "-...");

        Assert.Equal(
            [
                MorseElementKind.Dash, MorseElementKind.ElementGap,
                MorseElementKind.Dot, MorseElementKind.ElementGap,
                MorseElementKind.Dot, MorseElementKind.ElementGap,
                MorseElementKind.Dot, MorseElementKind.ElementGap,
            ],
            keyed.Select(element => element.Kind));
    }

    [Fact]
    public void WhatIsKeyedComesBackAsText()
    {
        IambicKeyer keyer = new(Wpm);
        KeyerDecoder decoder = new(Language.English, Wpm);
        StringBuilder copied = new();

        foreach (string character in new[] { "....", "..", " ", ".--.", "..-." })
        {
            if (character == " ")
            {
                decoder.Quiet(TimeSpan.FromMilliseconds(400));   // a word gap at 20 wpm is 420 ms
                Take(decoder, copied);
                continue;
            }

            foreach (MorseElement element in Key(keyer, character))
                decoder.Add(element);

            decoder.Quiet(TimeSpan.FromMilliseconds(150));       // a character gap is 180 ms
            Take(decoder, copied);
        }

        Assert.Equal("HI PF", copied.ToString());

        static void Take(KeyerDecoder decoder, StringBuilder into)
        {
            while (decoder.TryRead(out char character, out string? prosign))
                into.Append(prosign ?? character.ToString());
        }
    }

    [Fact]
    public void APauseTooShortToEndACharacterIsIgnored()
    {
        KeyerDecoder decoder = new(Language.English, Wpm);
        IambicKeyer keyer = new(Wpm);

        foreach (MorseElement element in Key(keyer, "."))
            decoder.Add(element);

        decoder.Quiet(TimeSpan.FromMilliseconds(60));    // one dit: still inside the character
        Assert.False(decoder.TryRead(out _, out _));

        decoder.Quiet(TimeSpan.FromMilliseconds(150));
        Assert.True(decoder.TryRead(out char character, out _));
        Assert.Equal('E', character);
    }

    [Fact]
    public void TheSamePauseReportedTwiceProducesOneCharacter()
    {
        KeyerDecoder decoder = new(Language.English, Wpm);
        IambicKeyer keyer = new(Wpm);

        foreach (MorseElement element in Key(keyer, ".-"))
            decoder.Add(element);

        decoder.Quiet(TimeSpan.FromMilliseconds(150));
        Assert.True(decoder.TryRead(out char character, out _));
        Assert.Equal('A', character);

        decoder.Quiet(TimeSpan.FromMilliseconds(160));
        Assert.False(decoder.TryRead(out _, out _));
    }

    [Fact]
    public void AProsignComesBackWhole()
    {
        KeyerDecoder decoder = new(Language.English, Wpm);
        IambicKeyer keyer = new(Wpm);

        // SK owns its pattern. AR and BT share theirs with punctuation, which keeps it for decoding.
        foreach (MorseElement element in Key(keyer, "...-.-"))
            decoder.Add(element);

        decoder.Quiet(TimeSpan.FromMilliseconds(150));
        Assert.True(decoder.TryRead(out _, out string? prosign));
        Assert.Equal("<SK>", prosign);
    }

    [Fact]
    public void FlushResolvesWhatIsLeft()
    {
        KeyerDecoder decoder = new(Language.English, Wpm);
        IambicKeyer keyer = new(Wpm);

        foreach (MorseElement element in Key(keyer, "-"))
            decoder.Add(element);

        Assert.True(decoder.Flush(out char character, out _));
        Assert.Equal('T', character);
        Assert.False(decoder.Flush(out _, out _));
    }

    [Fact]
    public void AnUnknownSequenceReadsAsAQuestionMark()
    {
        KeyerDecoder decoder = new(Language.English, Wpm);
        IambicKeyer keyer = new(Wpm);

        // Eight dots are the HH prosign, so this needs a sequence the alphabet really does not define.
        foreach (MorseElement element in Key(keyer, "--------"))
            decoder.Add(element);

        decoder.Quiet(TimeSpan.FromMilliseconds(150));
        Assert.True(decoder.TryRead(out char character, out _));
        Assert.Equal('?', character);
    }

    [Fact]
    public void ABadSpeedIsRejected()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new IambicKeyer(0));
        Assert.Throws<ArgumentOutOfRangeException>(() => new KeyerDecoder(Language.English, 0));
        Assert.Throws<ArgumentNullException>(() => new KeyerDecoder((MorseAlphabet)null!));
    }
}
