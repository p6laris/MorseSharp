using MorseSharp.Audio;

namespace MorseTest;

public class AudioFormatTests
{
    private const int Header = 44;

    private static byte[] Generate(AudioFormat format, string text = "PARIS") =>
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse(text)
            .ToAudio()
            .SetAudioOptions(20, 20, 700, format)
            .GetBytes();

    private static (ushort FormatTag, ushort Channels, uint SampleRate, uint ByteRate, ushort BlockAlign, ushort Bits) ReadHeader(byte[] wav)
    {
        ReadOnlySpan<byte> h = wav;
        return (
            BinaryPrimitives.ReadUInt16LittleEndian(h[20..]),
            BinaryPrimitives.ReadUInt16LittleEndian(h[22..]),
            BinaryPrimitives.ReadUInt32LittleEndian(h[24..]),
            BinaryPrimitives.ReadUInt32LittleEndian(h[28..]),
            BinaryPrimitives.ReadUInt16LittleEndian(h[32..]),
            BinaryPrimitives.ReadUInt16LittleEndian(h[34..]));
    }

    [Fact]
    public void TheDefaultIsWhatTheLibraryAlwaysProduced()
    {
        var header = ReadHeader(Generate(AudioFormat.Default));

        Assert.Equal(1, header.FormatTag);      // PCM
        Assert.Equal(1, header.Channels);       // mono
        Assert.Equal(11025u, header.SampleRate);
        Assert.Equal(16, header.Bits);
    }

    [Theory]
    [InlineData(8000)]
    [InlineData(11025)]
    [InlineData(22050)]
    [InlineData(44100)]
    [InlineData(48000)]
    public void TheSampleRateIsHonoured(int sampleRate)
    {
        byte[] wav = Generate(new AudioFormat(sampleRate));
        var header = ReadHeader(wav);

        Assert.Equal((uint)sampleRate, header.SampleRate);

        // A higher rate means proportionally more data for the same message.
        int frames = (wav.Length - Header) / header.BlockAlign;
        Assert.InRange(frames / (double)sampleRate, 1.0, 4.0);   // "PARIS" at 20 wpm is about 3 seconds
    }

    [Fact]
    public void StereoDoublesTheDataAndSaysSoInTheHeader()
    {
        byte[] mono = Generate(new AudioFormat(Channels: 1));
        byte[] stereo = Generate(new AudioFormat(Channels: 2));

        var header = ReadHeader(stereo);
        Assert.Equal(2, header.Channels);
        Assert.Equal(4, header.BlockAlign);                       // 2 channels x 16 bits
        Assert.Equal(11025u * 4, header.ByteRate);
        Assert.Equal((mono.Length - Header) * 2, stereo.Length - Header);
    }

    [Theory]
    [InlineData(AudioBitDepth.Pcm8, 1, 8, 1)]
    [InlineData(AudioBitDepth.Pcm16, 2, 16, 1)]
    [InlineData(AudioBitDepth.Float32, 4, 32, 3)]
    public void TheBitDepthIsHonoured(AudioBitDepth depth, int bytes, int bits, int formatTag)
    {
        byte[] wav = Generate(new AudioFormat(BitDepth: depth));
        var header = ReadHeader(wav);

        Assert.Equal((ushort)formatTag, header.FormatTag);        // 3 marks IEEE float
        Assert.Equal((ushort)bits, header.Bits);
        Assert.Equal((ushort)bytes, header.BlockAlign);
    }

    [Fact]
    public void EightBitHalvesTheFile()
    {
        int sixteen = Generate(new AudioFormat(BitDepth: AudioBitDepth.Pcm16)).Length - Header;
        int eight = Generate(new AudioFormat(BitDepth: AudioBitDepth.Pcm8)).Length - Header;
        Assert.Equal(sixteen / 2, eight);
    }

    [Fact]
    public void EightBitSilenceIsTheMidpointNotZero()
    {
        // Unsigned samples, so a buffer of zeros would be full-scale negative rather than quiet.
        byte[] wav = Generate(new AudioFormat(BitDepth: AudioBitDepth.Pcm8), "E");
        Assert.Equal(128, wav[^1]);
    }

    [Fact]
    public void FadingIsOnByDefaultAndRemovesTheStep()
    {
        static short LastSampleOfFirstElement(double edge)
        {
            byte[] wav = Morse.GetConverter().ForLanguage(Language.English).ToMorse("E").ToAudio()
                .SetAudioOptions(20, 20, 700, new AudioFormat(EdgeMilliseconds: edge)).GetBytes();

            ReadOnlySpan<short> pcm = MemoryMarshal.Cast<byte, short>(wav.AsSpan(Header));
            int last = 0;
            for (int i = 0; i < pcm.Length; i++)
            {
                if (pcm[i] != 0)
                    last = i;
            }

            return pcm[last];
        }

        Assert.True(Math.Abs(LastSampleOfFirstElement(0)) > 1000, "without a fade the tone should stop abruptly");
        Assert.True(Math.Abs(LastSampleOfFirstElement(5)) < 200, "with a fade the tone should die away");
        Assert.True(Math.Abs(LastSampleOfFirstElement(AudioFormat.Default.EdgeMilliseconds)) < 200, "fading should be the default");
    }

    [Fact]
    public void AudioStillDecodesAtEveryFormat()
    {
        // The decoder takes 16-bit mono, so this covers the sample rates rather than the depths.
        foreach (int sampleRate in new[] { 8000, 11025, 22050, 44100 })
        {
            byte[] wav = Generate(new AudioFormat(sampleRate), "HELLO WORLD");
            short[] pcm = MemoryMarshal.Cast<byte, short>(wav.AsSpan(Header)).ToArray();

            string decoded = Morse.GetConverter().ForLanguage(Language.English).FromAudio(pcm, sampleRate, 700, 20);
            Assert.Equal("HELLO WORLD", decoded);
        }
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ABadSampleRateIsRejected(int sampleRate)
        => Assert.Throws<ArgumentOutOfRangeException>(() => Generate(new AudioFormat(sampleRate)));

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public void ABadChannelCountIsRejected(int channels)
        => Assert.Throws<ArgumentOutOfRangeException>(() => Generate(new AudioFormat(Channels: channels)));

    [Fact]
    public void ANegativeFadeIsRejected()
        => Assert.Throws<ArgumentOutOfRangeException>(() => Generate(new AudioFormat(EdgeMilliseconds: -1)));

    [Fact]
    public void TheFrequencyLimitFollowsTheChosenSampleRate()
    {
        // 6 kHz is impossible at 11.025 kHz but fine at 44.1 kHz.
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Morse.GetConverter().ForLanguage(Language.English).ToMorse("E").ToAudio()
                .SetAudioOptions(20, 20, 6000, new AudioFormat(11025)).GetBytes());

        byte[] wav = Morse.GetConverter().ForLanguage(Language.English).ToMorse("E").ToAudio()
            .SetAudioOptions(20, 20, 6000, new AudioFormat(44100)).GetBytes();
        Assert.True(wav.Length > Header);
    }
}
