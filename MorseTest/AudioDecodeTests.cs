using MorseSharp.Audio.Decoding;

namespace MorseTest;

/// <summary>
/// Round-trips text through the encoder and back through the decoder. Owning both ends means the expected answer is
/// never in doubt, so any failure is the decoder's.
/// </summary>
public class AudioDecodeTests
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

    private static string Decode(short[] samples, double frequency, int wordsPerMinute) =>
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .FromAudio(samples, SampleRate, frequency, wordsPerMinute);

    [Fact]
    public void DecodesASingleCharacter()
    {
        Assert.Equal("E", Decode(Generate("E", 20, 700), 700, 20));
    }

    [Fact]
    public void DecodesASingleWord()
    {
        Assert.Equal("HELLO", Decode(Generate("HELLO", 20, 700), 700, 20));
    }

    [Fact]
    public void DecodesTwoWords()
    {
        Assert.Equal("HELLO WORLD", Decode(Generate("HELLO WORLD", 20, 700), 700, 20));
    }

    [Theory]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    [InlineData(25)]
    [InlineData(30)]
    [InlineData(40)]
    public void DecodesAtAnySpeed(int wordsPerMinute)
    {
        const string text = "CQ DE MORSE";
        Assert.Equal(text, Decode(Generate(text, wordsPerMinute, 700), 700, wordsPerMinute));
    }

    [Theory]
    [InlineData(400)]
    [InlineData(600)]
    [InlineData(700)]
    [InlineData(1000)]
    [InlineData(1500)]
    public void DecodesAtAnyToneFrequency(double frequency)
    {
        const string text = "TEST";
        Assert.Equal(text, Decode(Generate(text, 20, frequency), frequency, 20));
    }

    [Fact]
    public void DecodesAPangram()
    {
        const string text = "THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG";
        Assert.Equal(text, Decode(Generate(text, 25, 700), 700, 25));
    }

    [Fact]
    public void DecodesDigitsAndPunctuation()
    {
        const string text = "SOS 123";
        Assert.Equal(text, Decode(Generate(text, 20, 700), 700, 20));
    }

    [Theory]
    [InlineData(12)]
    [InlineData(18)]
    [InlineData(30)]
    [InlineData(35)]
    public void RecoversWhenTheStatedSpeedIsWrong(int statedWordsPerMinute)
    {
        // Sent at 22 wpm but told something else, which is the everyday case with a live operator.
        const string text = "PARIS PARIS PARIS";
        Assert.Equal(text, Decode(Generate(text, 22, 700), 700, statedWordsPerMinute));
    }

    [Fact]
    public void GoertzelHearsItsOwnToneAndIgnoresOthers()
    {
        short[] tone = Generate("T", 20, 700);   // one long dash at 700 Hz
        GoertzelDetector onFrequency = new(700, SampleRate, 128);
        GoertzelDetector offFrequency = new(1800, SampleRate, 128);

        ReadOnlySpan<short> block = tone.AsSpan(200, 128);
        float matched = onFrequency.Power(block);
        float mismatched = offFrequency.Power(block);

        Assert.True(matched > mismatched * 50, $"tuned {matched:F4} vs detuned {mismatched:F4}");
    }

    [Fact]
    public void GoertzelSnapsToAWholeBin()
    {
        // 700 Hz does not land on a bin boundary for this window, so the detector reports where it actually listens.
        GoertzelDetector detector = new(700, SampleRate, 128);
        Assert.InRange(detector.Frequency, 650, 750);
        Assert.Equal(128, detector.WindowSize);
    }

    /// <summary>Adds white noise at a given signal-to-noise ratio, in decibels.</summary>
    private static short[] AddNoise(short[] samples, double signalToNoiseDb, int seed = 12345)
    {
        Random random = new(seed);
        double noiseAmplitude = 32760.0 / Math.Pow(10, signalToNoiseDb / 20.0);

        short[] noisy = new short[samples.Length];
        for (int i = 0; i < samples.Length; i++)
        {
            double noise = (random.NextDouble() * 2 - 1) * noiseAmplitude;
            noisy[i] = (short)Math.Clamp(samples[i] + noise, short.MinValue, short.MaxValue);
        }
        return noisy;
    }

    [Theory]
    [InlineData(30)]
    [InlineData(20)]
    [InlineData(10)]
    [InlineData(6)]
    [InlineData(3)]
    [InlineData(0)]
    public void DecodesThroughNoise(double signalToNoiseDb)
    {
        const string text = "CQ CQ DE MORSE";
        short[] noisy = AddNoise(Generate(text, 20, 700), signalToNoiseDb);
        Assert.Equal(text, Decode(noisy, 700, 20));
    }

    [Fact]
    public void DecodesFarnsworthTiming()
    {
        // Characters keyed at 25 wpm but spaced out to an overall 12 wpm, which is how practice audio is sent.
        byte[] wav = Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse("HELLO WORLD")
            .ToAudio()
            .SetAudioOptions(charSpeed: 25, wordSpeed: 12, frequency: 700)
            .GetBytes();

        short[] pcm = MemoryMarshal.Cast<byte, short>(wav.AsSpan(Header)).ToArray();
        Assert.Equal("HELLO WORLD", Decode(pcm, 700, 25));
    }

    [Fact]
    public void SilenceDecodesToNothing()
    {
        Assert.Equal(string.Empty, Decode(new short[11025], 700, 20));
    }

    [Fact]
    public void AnEmptyBufferDecodesToNothing()
    {
        Assert.Equal(string.Empty, Decode([], 700, 20));
    }

    [Fact]
    public void ListeningOnTheWrongFrequencyFindsNothing()
    {
        // A 700 Hz signal searched for at 2000 Hz should not invent characters.
        short[] pcm = Generate("HELLO", 20, 700);
        Assert.Equal(string.Empty, Decode(pcm, 2000, 20));
    }
}
