using MorseSharp.Audio;

namespace MorseTest;

public class ToneGeneratorTests
{
    private const int SampleRate = 11025;

    /// <summary>The straightforward implementation the vectorised one has to agree with.</summary>
    private static short[] Reference(int length, double frequency)
    {
        short[] expected = new short[length];
        double increment = 2 * Math.PI * frequency / SampleRate;
        for (int i = 0; i < length; i++)
            expected[i] = (short)(ToneGenerator.Amplitude * Math.Sin(i * increment));
        return expected;
    }

    [Theory]
    [InlineData(1, 700)]
    [InlineData(7, 700)]
    [InlineData(16, 700)]
    [InlineData(17, 700)]
    [InlineData(529, 700)]     // one dot at 25 wpm
    [InlineData(1587, 700)]    // one dash at 25 wpm
    [InlineData(7938, 600)]    // one dash at 5 wpm, past the stackalloc limit
    [InlineData(4096, 5000)]   // close to Nyquist
    [InlineData(4096, 1)]      // very low frequency
    public void MatchesTheScalarReference(int length, double frequency)
    {
        short[] actual = new short[length];
        ToneGenerator.Fill(actual, frequency, SampleRate);

        short[] expected = Reference(length, frequency);
        for (int i = 0; i < length; i++)
        {
            // Rotation instead of a sine call per sample; one least significant bit of slack.
            Assert.True(Math.Abs(actual[i] - expected[i]) <= 1, $"sample {i}: expected {expected[i]}, got {actual[i]}");
        }
    }

    [Fact]
    public void StartsAtZeroAndStaysInRange()
    {
        short[] tone = new short[8192];
        ToneGenerator.Fill(tone, 700, SampleRate);

        Assert.Equal(0, tone[0]);
        Assert.All(tone, sample => Assert.InRange(sample, -ToneGenerator.Amplitude, ToneGenerator.Amplitude));

        // Samples rarely land exactly on a peak, so allow a little slack on both extremes.
        Assert.Contains(tone, sample => sample > ToneGenerator.Amplitude - 100);
        Assert.Contains(tone, sample => sample < -ToneGenerator.Amplitude + 100);
    }

    [Fact]
    public void DoesNotDriftOverALongTone()
    {
        // The vector rotation must not lose amplitude by the end of the longest tone the library can produce.
        const int length = 40000;
        short[] tone = new short[length];
        ToneGenerator.Fill(tone, 700, SampleRate);

        short[] expected = Reference(length, 700);
        for (int i = length - 64; i < length; i++)
            Assert.True(Math.Abs(tone[i] - expected[i]) <= 1, $"sample {i}: expected {expected[i]}, got {tone[i]}");
    }
}
