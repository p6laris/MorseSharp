namespace MorseTest;

public class AudioTests
{
    // At 25 wpm and 11025 Hz: dit = 48 ms = 529 samples, character gap = 3 dits = 1588 samples, word gap = 7 dits = 3704 samples.
    private const int Dot25 = 529;
    private const int CharGap25 = 1588;
    private const int WordGap25 = 3704;
    private const int Header = 44;

    private static ICanConvertToAudio Audio(string text, int charSpeed = 25, int wordSpeed = 25, double frequency = 700) =>
        Morse.GetConverter().ForLanguage(Language.English).ToMorse(text).ToAudio().SetAudioOptions(charSpeed, wordSpeed, frequency);

    private static int Samples(string text, int charSpeed = 25, int wordSpeed = 25) =>
        (Audio(text, charSpeed, wordSpeed).GetByteCount() - Header) / 2;

    [Fact]
    public void HeaderIsCanonicalPcm()
    {
        byte[] wav = Audio("E").GetBytes();
        ReadOnlySpan<byte> span = wav;

        Assert.Equal("RIFF", Encoding.ASCII.GetString(span[..4]));
        Assert.Equal(wav.Length - 8, BinaryPrimitives.ReadInt32LittleEndian(span[4..]));
        Assert.Equal("WAVE", Encoding.ASCII.GetString(span[8..12]));
        Assert.Equal("fmt ", Encoding.ASCII.GetString(span[12..16]));
        Assert.Equal(16, BinaryPrimitives.ReadInt32LittleEndian(span[16..]));
        Assert.Equal(1, BinaryPrimitives.ReadInt16LittleEndian(span[20..]));     // PCM
        Assert.Equal(1, BinaryPrimitives.ReadInt16LittleEndian(span[22..]));     // mono
        Assert.Equal(11025, BinaryPrimitives.ReadInt32LittleEndian(span[24..])); // sample rate
        Assert.Equal(22050, BinaryPrimitives.ReadInt32LittleEndian(span[28..])); // byte rate
        Assert.Equal(2, BinaryPrimitives.ReadInt16LittleEndian(span[32..]));     // block align
        Assert.Equal(16, BinaryPrimitives.ReadInt16LittleEndian(span[34..]));    // bits per sample
        Assert.Equal("data", Encoding.ASCII.GetString(span[36..40]));
        Assert.Equal(wav.Length - Header, BinaryPrimitives.ReadInt32LittleEndian(span[40..]));
    }

    [Fact]
    public void SingleDotHasStandardTiming()
    {
        byte[] wav = Audio("E").GetBytes();
        ReadOnlySpan<short> pcm = MemoryMarshal.Cast<byte, short>(wav.AsSpan(Header));

        Assert.Equal(Dot25 + CharGap25, pcm.Length);
        Assert.Contains(pcm[..Dot25].ToArray(), sample => sample != 0);
        Assert.All(pcm[Dot25..].ToArray(), sample => Assert.Equal(0, sample));
    }

    [Fact]
    public void GapsFollowThreeAndSevenUnits()
    {
        Assert.Equal(2 * Dot25 + 2 * CharGap25, Samples("EE"));
        Assert.Equal(2 * Dot25 + WordGap25 + CharGap25, Samples("E E"));
        Assert.Equal(3 * Dot25 + CharGap25, Samples("T")); // a dash is exactly three dots
    }

    [Fact]
    public void FarnsworthStretchesOnlyTheGaps()
    {
        // ta = 60/10 - 37.2/25 = 4.512 s; character gap = 3 * ta / 19 = 0.7124 s = 7854 samples.
        Assert.Equal(2 * Dot25 + 2 * 7854, Samples("EE", 25, 10));
        Assert.True(Samples("EE", 25, 10) > Samples("EE", 25, 25));
    }

    [Fact]
    public void ByteCountMatchesEveryOutputPath()
    {
        ICanConvertToAudio audio = Audio("Hello World");
        int count = audio.GetByteCount();

        byte[] array = audio.GetBytes();
        Assert.Equal(count, array.Length);

        byte[] buffer = new byte[count + 10];
        Assert.Equal(count, audio.GetBytes(buffer));
        Assert.True(array.AsSpan().SequenceEqual(buffer.AsSpan(0, count)));

        using MemoryStream stream = new();
        audio.WriteTo(stream);
        Assert.Equal(array, stream.ToArray());
    }

    [Fact]
    public void TooSmallDestinationThrows()
    {
        ICanConvertToAudio audio = Audio("Hello");
        var ex = Assert.Throws<ArgumentException>(() => audio.GetBytes(new byte[10]));
        Assert.Equal("destination", ex.ParamName);
    }

    [Fact]
    public void ManualMorseProducesIdenticalAudioToEncodedText()
    {
        byte[] fromText = Audio("Hello World").GetBytes();
        byte[] fromMorse = Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToAudio(".... . .-.. .-.. --- / .-- --- .-. .-.. -..")
            .SetAudioOptions(25, 25, 700)
            .GetBytes();

        Assert.Equal(fromText, fromMorse);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(5512.5)]
    [InlineData(6000)]
    [InlineData(double.NaN)]
    public void InvalidFrequencyThrows(double frequency)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Audio("E", 25, 25, frequency));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(0, 25)]
    [InlineData(25, 0)]
    [InlineData(-1, -1)]
    public void NonPositiveSpeedThrows(int charSpeed, int wordSpeed)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => Audio("E", charSpeed, wordSpeed));
    }

    [Fact]
    public void InvalidMorseSymbolThrows()
    {
        var ex = Assert.Throws<ArgumentException>(() => Morse.GetConverter().ForLanguage(Language.English).ToAudio(".... x .."));
        Assert.Equal("morse", ex.ParamName);
    }

    [Fact]
    public void ToneIsAtTheRequestedFrequency()
    {
        // A dash at 25 wpm lasts 0.144 s; at 700 Hz that is 100.8 cycles, so about 201 zero crossings.
        byte[] wav = Audio("T", 25, 25, 700).GetBytes();
        ReadOnlySpan<short> dash = MemoryMarshal.Cast<byte, short>(wav.AsSpan(Header, 3 * Dot25 * 2));

        int crossings = 0;
        for (int i = 1; i < dash.Length; i++)
        {
            if ((dash[i - 1] < 0) != (dash[i] < 0))
                crossings++;
        }

        Assert.InRange(crossings, 199, 203);
    }

    [Fact]
    public void LowSpeedUsesThePooledToneBuffer()
    {
        // 5 wpm: dit = 0.24 s = 2646 samples, dash = 7938 samples (above the stackalloc limit), character gap = 7938 samples.
        byte[] wav = Audio("T", 5, 5).GetBytes();
        Assert.Equal(Header + 2 * (7938 + 7938), wav.Length);
    }

    [Fact]
    public void ObsoleteSpanOverloadReturnsAFullFile()
    {
#pragma warning disable CS0618
        Audio("E").GetBytes(out Span<byte> span);
#pragma warning restore CS0618
        Assert.Equal(Audio("E").GetByteCount(), span.Length);
        Assert.Equal("RIFF", Encoding.ASCII.GetString(span[..4]));
    }

    [Fact]
    public void LeadingAndTrailingSpacesBecomeSilence()
    {
        Assert.Equal(Dot25 + 2 * WordGap25 + CharGap25, Samples(" E "));
    }
}
