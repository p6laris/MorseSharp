using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using MorseSharp;

namespace Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class MorseSharpBenchmarks
{
    private static readonly string ShortText = "Hi";
    private static readonly string MediumText = "Hello World";
    private static readonly string LongText = "The quick brown fox jumps over the lazy dog";

    private static readonly string ShortMorse = ".... ..";
    private static readonly string MediumMorse = ".... . .-.. .-.. --- / .-- --- .-. .-.. -..";
    private static readonly string LongMorse = "- .... . / --.- ..- .. -.-. -.- / -... .-. --- .-- -. / ..-. --- -..- / .--- ..- -- .--. ... / --- ...- . .-. / - .... . / .-.. .- --.. -.-- / -.. --- --.";

    [Benchmark(Baseline = true, Description = "Construct Morse Converter")]
    public void ConstructConverter()
    {
        _ = Morse.GetConverter().ForLanguage(Language.English);
    }

    [Benchmark(Description = "Encode Short: Hi")]
    public void EncodeShort()
    {
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse(ShortText)
            .Encode();
    }

    [Benchmark(Description = "Encode Medium: Hello World")]
    public void EncodeMedium()
    {
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse(MediumText)
            .Encode();
    }

    [Benchmark(Description = "Encode Long: Pangram")]
    public void EncodeLong()
    {
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse(LongText)
            .Encode();
    }

    [Benchmark(Description = "Decode Short: Hi")]
    public void DecodeShort()
    {
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .Decode(ShortMorse);
    }

    [Benchmark(Description = "Decode Medium: Hello World")]
    public void DecodeMedium()
    {
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .Decode(MediumMorse);
    }

    [Benchmark(Description = "Decode Long: Pangram")]
    public void DecodeLong()
    {
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .Decode(LongMorse);
    }

    [Benchmark(Description = "Audio Short: Hi")]
    public void AudioShort()
    {
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToAudio(ShortMorse)
            .SetAudioOptions(25, 25, 600)
            .GetBytes(out Span<byte> _);
    }

    [Benchmark(Description = "Audio Medium: Hello World")]
    public void AudioMedium()
    {
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToAudio(MediumMorse)
            .SetAudioOptions(25, 25, 600)
            .GetBytes(out Span<byte> _);
    }

    [Benchmark(Description = "Audio Long: Pangram)")]
    public void AudioLong()
    {
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToAudio(LongMorse)
            .SetAudioOptions(25, 25, 600)
            .GetBytes(out Span<byte> _);
    }
}
