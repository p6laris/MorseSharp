using MorseSharp.Audio;

namespace MorseTest;

public class ToneGeneratorTests
{
    private const int SampleRate = 11025;

    private static short[] RenderPcm16(int samples, double frequency, double edgeMilliseconds)
    {
        AudioFormat format = new(SampleRate, Channels: 1, AudioBitDepth.Pcm16, edgeMilliseconds);
        byte[] bytes = new byte[samples * format.BytesPerFrame];
        ToneGenerator.Render(bytes, samples, frequency, format);
        return MemoryMarshal.Cast<byte, short>(bytes).ToArray();
    }

    [Theory]
    [InlineData(1, 700)]
    [InlineData(16, 700)]
    [InlineData(529, 700)]   // one dot at 25 wpm
    [InlineData(1587, 700)]  // one dash at 25 wpm
    [InlineData(4096, 5000)] // close to Nyquist
    [InlineData(4096, 1)]    // very low frequency
    public void WithoutAFadeItMatchesAPlainSine(int samples, double frequency)
    {
        short[] actual = RenderPcm16(samples, frequency, edgeMilliseconds: 0);

        double increment = 2 * Math.PI * frequency / SampleRate;
        for (int i = 0; i < samples; i++)
        {
            short expected = (short)(ToneGenerator.Amplitude * Math.Sin(i * increment) * short.MaxValue);

            // Rotation instead of a sine call per sample, so allow a little accumulated slack.
            Assert.True(Math.Abs(actual[i] - expected) <= 2, $"sample {i}: expected {expected}, got {actual[i]}");
        }
    }

    [Fact]
    public void AFadeStartsAndEndsAtSilence()
    {
        short[] tone = RenderPcm16(1587, 700, edgeMilliseconds: 5);

        Assert.Equal(0, tone[0]);
        Assert.Equal(0, tone[^1]);
    }

    [Fact]
    public void AFadeRemovesTheJumpAtTheEnd()
    {
        // Without a fade the waveform stops wherever it happens to be, and that step is the key click.
        short[] hard = RenderPcm16(1587, 700, edgeMilliseconds: 0);
        short[] faded = RenderPcm16(1587, 700, edgeMilliseconds: 5);

        Assert.True(Math.Abs(hard[^1]) > 1000, $"expected an abrupt ending, got {hard[^1]}");
        Assert.True(Math.Abs(faded[^1]) < 50, $"expected a gentle ending, got {faded[^1]}");
    }

    [Fact]
    public void TheFadeRisesMonotonically()
    {
        // Measured as a peak envelope, since the sine itself oscillates within the ramp.
        short[] tone = RenderPcm16(4410, 100, edgeMilliseconds: 20);
        int edge = (int)(0.020 * SampleRate);

        short previous = 0;
        for (int start = 0; start + 110 <= edge; start += 110)
        {
            short peak = 0;
            for (int i = start; i < start + 110; i++)
                peak = Math.Max(peak, Math.Abs(tone[i]));

            Assert.True(peak >= previous, $"envelope fell from {previous} to {peak} at sample {start}");
            previous = peak;
        }
    }

    [Fact]
    public void AFadeLongerThanTheElementIsTrimmedToFit()
    {
        // 500 ms of fade on a 10 ms element would otherwise have the two ends overlapping.
        short[] tone = RenderPcm16(110, 700, edgeMilliseconds: 500);
        Assert.Equal(110, tone.Length);
        Assert.All(tone, sample => Assert.InRange(sample, short.MinValue, short.MaxValue));
    }

    [Fact]
    public void StereoCarriesTheSameToneInBothChannels()
    {
        AudioFormat format = new(SampleRate, Channels: 2, AudioBitDepth.Pcm16, EdgeMilliseconds: 0);
        byte[] bytes = new byte[100 * format.BytesPerFrame];
        ToneGenerator.Render(bytes, 100, 700, format);

        ReadOnlySpan<short> samples = MemoryMarshal.Cast<byte, short>(bytes);
        for (int frame = 0; frame < 100; frame++)
            Assert.Equal(samples[frame * 2], samples[(frame * 2) + 1]);
    }

    [Fact]
    public void EightBitSilenceSitsAtTheMidpoint()
    {
        // 8-bit WAV samples are unsigned, so quiet is 128 rather than 0.
        AudioFormat format = new(SampleRate, Channels: 1, AudioBitDepth.Pcm8, EdgeMilliseconds: 5);
        byte[] bytes = new byte[1587];
        ToneGenerator.Render(bytes, 1587, 700, format);

        Assert.Equal(128, bytes[0]);
        Assert.Equal(128, format.SilenceByte);
    }
}
