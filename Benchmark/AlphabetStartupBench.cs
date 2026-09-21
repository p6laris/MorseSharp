using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using MorseSharp;
using MorseSharp.Alphabet;
using MorseSharp.Characters;

namespace Benchmark;

/// <summary>
/// Isolates what source generation actually changed: how an alphabet comes into existence.
/// </summary>
/// <remarks>
/// <c>Pack</c> is the work startup used to do, running the hash and probe loops over every character. <c>FromBlobs</c>
/// is what it does now, copying tables the compiler already baked into the assembly. Steady-state lookups are
/// untouched by this, because both paths end up with identical arrays.
/// </remarks>
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class AlphabetStartupBenchmarks
{
    private readonly List<MorseEntry> _englishEntries = [.. Alphabets.English.Entries];
    private readonly List<MorseEntry> _russianEntries = [.. Alphabets.Russian.Entries];
    private readonly List<MorseEntry> _japaneseEntries = [.. Alphabets.Japanese.Entries];

    [Benchmark(Baseline = true, Description = "Pack English at run time (the old startup path)")]
    public MorseAlphabet PackEnglish() => MorseAlphabet.Pack("English", _englishEntries);

    [Benchmark(Description = "Load English from blobs (the new startup path)")]
    public MorseAlphabet BlobEnglish() => MorseAlphabet.FromBlobs(
        "English", EnglishCharacters.AsciiBlob, EnglishCharacters.HashedBlob, EnglishCharacters.DecodeBlob,
        EnglishCharacters.MaxProbeLength, EnglishCharacters.DecodableCount, EnglishCharacters.CreateEntries);

    [Benchmark(Description = "Pack Russian at run time (largest table)")]
    public MorseAlphabet PackRussian() => MorseAlphabet.Pack("Russian", _russianEntries);

    [Benchmark(Description = "Load Russian from blobs")]
    public MorseAlphabet BlobRussian() => MorseAlphabet.FromBlobs(
        "Russian", RussianCharacters.AsciiBlob, RussianCharacters.HashedBlob, RussianCharacters.DecodeBlob,
        RussianCharacters.MaxProbeLength, RussianCharacters.DecodableCount, RussianCharacters.CreateEntries);

    [Benchmark(Description = "Pack Japanese at run time (most entries)")]
    public MorseAlphabet PackJapanese() => MorseAlphabet.Pack("Japanese", _japaneseEntries);

    [Benchmark(Description = "Load Japanese from blobs")]
    public MorseAlphabet BlobJapanese() => MorseAlphabet.FromBlobs(
        "Japanese", JapaneseCharacters.AsciiBlob, JapaneseCharacters.HashedBlob, JapaneseCharacters.DecodeBlob,
        JapaneseCharacters.MaxProbeLength, JapaneseCharacters.DecodableCount, JapaneseCharacters.CreateEntries);
}
