using System.Text;

namespace MorseTest;

/// <summary>
/// Feeds the same audio through the streaming decoder in pieces. Chunk sizes are deliberately awkward, because a
/// caller's buffer never lines up with the decoder's analysis blocks.
/// </summary>
public class StreamingDecodeTests
{
    private const int SampleRate = 11025;
    private const int Header = 44;

    private static short[] Generate(string text, int wordsPerMinute, double frequency)
    {
        byte[] wav = Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse(text)
            .ToAudio()
            .SetAudioOptions(wordsPerMinute, wordsPerMinute, frequency)
            .GetBytes();

        return MemoryMarshal.Cast<byte, short>(wav.AsSpan(Header)).ToArray();
    }

    private static string StreamDecode(short[] samples, int chunkSize, double frequency = 700, int wordsPerMinute = 20)
    {
        StreamingMorseDecoder decoder = Morse.GetConverter()
            .ForLanguage(Language.English)
            .CreateAudioDecoder(SampleRate, frequency, wordsPerMinute);

        StringBuilder text = new();
        for (int offset = 0; offset < samples.Length; offset += chunkSize)
        {
            int take = Math.Min(chunkSize, samples.Length - offset);
            decoder.Write(samples.AsSpan(offset, take));

            while (decoder.TryRead(out char character))
                text.Append(character);
        }

        decoder.Flush();
        while (decoder.TryRead(out char character))
            text.Append(character);

        return text.ToString().Trim();
    }

    [Theory]
    [InlineData(1)]        // one sample at a time, the worst case for the carry buffer
    [InlineData(7)]        // prime, never aligns with the block size
    [InlineData(64)]
    [InlineData(100)]
    [InlineData(441)]
    [InlineData(4096)]
    [InlineData(1 << 20)]  // larger than the whole signal
    public void DecodesRegardlessOfChunkSize(int chunkSize)
    {
        const string text = "HELLO WORLD";
        Assert.Equal(text, StreamDecode(Generate(text, 20, 700), chunkSize));
    }

    [Fact]
    public void StreamingAgreesWithWholeBufferDecoding()
    {
        const string text = "THE QUICK BROWN FOX";
        short[] samples = Generate(text, 20, 700);

        string batch = Morse.GetConverter().ForLanguage(Language.English).FromAudio(samples, SampleRate, 700, 20);
        Assert.Equal(batch, StreamDecode(samples, 512));
    }

    [Theory]
    [InlineData(12)]
    [InlineData(20)]
    [InlineData(30)]
    public void DecodesAtAnySpeed(int wordsPerMinute)
    {
        const string text = "CQ DE MORSE";
        Assert.Equal(text, StreamDecode(Generate(text, wordsPerMinute, 700), 512, 700, wordsPerMinute));
    }

    [Fact]
    public void KeepsTheFirstCharacter()
    {
        // The opening blocks arrive before anything is known about the signal, so they are replayed once it is.
        Assert.Equal("EEE", StreamDecode(Generate("EEE", 20, 700), 256));
    }

    [Fact]
    public void WithoutFlushTheLastCharacterIsStillPending()
    {
        // A character is only known to be complete once a gap follows it.
        StreamingMorseDecoder decoder = Morse.GetConverter()
            .ForLanguage(Language.English)
            .CreateAudioDecoder(SampleRate, 700, 20);

        decoder.Write(Generate("HI", 20, 700));

        StringBuilder beforeFlush = new();
        while (decoder.TryRead(out char c))
            beforeFlush.Append(c);

        decoder.Flush();
        StringBuilder afterFlush = new(beforeFlush.ToString());
        while (decoder.TryRead(out char c))
            afterFlush.Append(c);

        Assert.Equal("H", beforeFlush.ToString());
        Assert.Equal("HI", afterFlush.ToString());
    }

    [Fact]
    public void ResetClearsEverything()
    {
        StreamingMorseDecoder decoder = Morse.GetConverter()
            .ForLanguage(Language.English)
            .CreateAudioDecoder(SampleRate, 700, 20);

        decoder.Write(Generate("HELLO", 20, 700));
        decoder.Reset();
        Assert.False(decoder.TryRead(out _));

        decoder.Write(Generate("SOS", 20, 700));
        decoder.Flush();

        StringBuilder text = new();
        while (decoder.TryRead(out char c))
            text.Append(c);

        Assert.Equal("SOS", text.ToString().Trim());
    }

    [Fact]
    public void SilenceProducesNothing()
    {
        Assert.Equal(string.Empty, StreamDecode(new short[SampleRate * 2], 512));
    }

    [Fact]
    public void RejectsBadArguments()
    {
        var conv = Morse.GetConverter().ForLanguage(Language.English);
        Assert.Throws<ArgumentOutOfRangeException>(() => conv.CreateAudioDecoder(0, 700, 20));
        Assert.Throws<ArgumentOutOfRangeException>(() => conv.CreateAudioDecoder(SampleRate, 700, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => conv.CreateAudioDecoder(SampleRate, 0, 20));
        Assert.Throws<ArgumentOutOfRangeException>(() => conv.CreateAudioDecoder(SampleRate, SampleRate, 20));
    }
}
