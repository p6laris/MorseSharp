using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using MorseSharp;

namespace Benchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[RankColumn]
public class MorseSharpBenchmarks
{
    private const string ShortText = "Hi";
    private const string MediumText = "Hello World";
    private const string LongText = "The quick brown fox jumps over the lazy dog";
    private const string KurdishText = "کۆژین و ڤیان چوونە بۆ باغەکە";

    private const string ShortMorse = ".... ..";
    private const string MediumMorse = ".... . .-.. .-.. --- / .-- --- .-. .-.. -..";
    private const string LongMorse = "- .... . / --.- ..- .. -.-. -.- / -... .-. --- .-- -. / ..-. --- -..- / .--- ..- -- .--. ... / --- ...- . .-. / - .... . / .-.. .- --.. -.-- / -.. --- --.";
    private const string KurdishMorse = "-.-.. .-.- --. .. -. / .-- / ..-.. .. .- -. / ---. .-- .-- -. . / -... .-.- / -... .- ..-- . -.-.. .";

    private readonly byte[] _wavBuffer = new byte[1 << 20];

    [Benchmark(Baseline = true, Description = "Construct Morse Converter")]
    public void ConstructConverter()
    {
        _ = Morse.GetConverter().ForLanguage(Language.English);
    }

    [Benchmark(Description = "Encode Short: Hi")]
    public string EncodeShort() => Morse.GetConverter().ForLanguage(Language.English).ToMorse(ShortText).Encode();

    [Benchmark(Description = "Encode Medium: Hello World")]
    public string EncodeMedium() => Morse.GetConverter().ForLanguage(Language.English).ToMorse(MediumText).Encode();

    [Benchmark(Description = "Encode Long: Pangram")]
    public string EncodeLong() => Morse.GetConverter().ForLanguage(Language.English).ToMorse(LongText).Encode();

    [Benchmark(Description = "Encode Kurdish")]
    public string EncodeKurdish() => Morse.GetConverter().ForLanguage(Language.Kurdish).ToMorse(KurdishText).Encode();

    [Benchmark(Description = "Decode Short: Hi")]
    public string DecodeShort() => Morse.GetConverter().ForLanguage(Language.English).Decode(ShortMorse);

    [Benchmark(Description = "Decode Medium: Hello World")]
    public string DecodeMedium() => Morse.GetConverter().ForLanguage(Language.English).Decode(MediumMorse);

    [Benchmark(Description = "Decode Long: Pangram")]
    public string DecodeLong() => Morse.GetConverter().ForLanguage(Language.English).Decode(LongMorse);

    [Benchmark(Description = "Decode Kurdish")]
    public string DecodeKurdish() => Morse.GetConverter().ForLanguage(Language.Kurdish).Decode(KurdishMorse);

    [Benchmark(Description = "Audio Short: Hi")]
    public byte[] AudioShort() => Morse.GetConverter().ForLanguage(Language.English).ToAudio(ShortMorse).SetAudioOptions(25, 25, 600).GetBytes();

    [Benchmark(Description = "Audio Medium: Hello World")]
    public byte[] AudioMedium() => Morse.GetConverter().ForLanguage(Language.English).ToAudio(MediumMorse).SetAudioOptions(25, 25, 600).GetBytes();

    [Benchmark(Description = "Audio Long: Pangram")]
    public byte[] AudioLong() => Morse.GetConverter().ForLanguage(Language.English).ToAudio(LongMorse).SetAudioOptions(25, 25, 600).GetBytes();

    [Benchmark(Description = "Audio Long: Pangram from text")]
    public byte[] AudioLongFromText() => Morse.GetConverter().ForLanguage(Language.English).ToMorse(LongText).ToAudio().SetAudioOptions(25, 25, 600).GetBytes();

    [Benchmark(Description = "Audio Long: Pangram into caller buffer")]
    public int AudioLongIntoBuffer() => Morse.GetConverter().ForLanguage(Language.English).ToAudio(LongMorse).SetAudioOptions(25, 25, 600).GetBytes(_wavBuffer);

    [Benchmark(Description = "Audio Long: Pangram byte count")]
    public int AudioLongByteCount() => Morse.GetConverter().ForLanguage(Language.English).ToAudio(LongMorse).SetAudioOptions(25, 25, 600).GetByteCount();
}
