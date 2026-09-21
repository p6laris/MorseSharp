using MorseSharp.Alphabet;

namespace MorseTest;

/// <summary>
/// Guards the contract that makes build-time packing safe: the tables baked into the assembly must be exactly what
/// the runtime packer would have produced. If someone tunes the hash or the insertion order and only one side
/// changes, these fail immediately rather than shipping alphabets that quietly disagree.
/// </summary>
public class GeneratedAlphabetTests
{
    public static TheoryData<Language> AllLanguages
    {
        get
        {
            TheoryData<Language> data = [];
            foreach (Language language in Enum.GetValues<Language>())
                data.Add(language);
            return data;
        }
    }

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void GeneratedTablesMatchTheRuntimePacker(Language language)
    {
        MorseAlphabet generated = Alphabets.For(language);
        MorseAlphabet runtime = MorseAlphabet.Pack(generated.Name, [.. generated.Entries]);

        Assert.Equal(runtime.HashedSlots, generated.HashedSlots);
        Assert.Equal(runtime.MaxProbeLength, generated.MaxProbeLength);
        Assert.Equal(runtime.DecodableCount, generated.DecodableCount);

        // Every character in the BMP must resolve identically through both tables.
        for (int i = 0; i <= char.MaxValue; i++)
        {
            char ch = (char)i;
            bool generatedFound = generated.TryGetCode(ch, out int generatedCode);
            bool runtimeFound = runtime.TryGetCode(ch, out int runtimeCode);

            if (generatedFound != runtimeFound || generatedCode != runtimeCode)
            {
                Assert.Fail(
                    $"{language} disagrees on U+{i:X4}: generated {(generatedFound ? generatedCode.ToString() : "absent")}, " +
                    $"runtime {(runtimeFound ? runtimeCode.ToString() : "absent")}");
            }
        }

        // And every tree code must decode identically.
        for (int code = 0; code < MorseAlphabet.CodeLimit; code++)
            Assert.Equal(runtime.Decode(code), generated.Decode(code));
    }

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void GeneratedEntriesSurviveARoundTripThroughTheBuilder(Language language)
    {
        // Rebuilding an alphabet from its own entries must be a no-op, which is what From(Language) relies on.
        MorseAlphabet original = Alphabets.For(language);
        MorseAlphabet rebuilt = MorseAlphabetBuilder.From(language).Build();

        Assert.Equal(original.Name, rebuilt.Name);
        Assert.Equal(original.HashedSlots, rebuilt.HashedSlots);
        Assert.Equal(original.DecodableCount, rebuilt.DecodableCount);

        for (int i = 0; i <= char.MaxValue; i++)
        {
            char ch = (char)i;
            Assert.Equal(original.TryGetCode(ch, out int a), rebuilt.TryGetCode(ch, out int b));
            Assert.Equal(a, b);
        }
    }

    [Theory]
    [MemberData(nameof(AllLanguages))]
    public void EveryLanguageHasEntries(Language language)
    {
        // A .morse file silently failing to parse would leave an alphabet that builds but knows nothing.
        MorseAlphabet alphabet = Alphabets.For(language);
        Assert.NotEmpty(alphabet.Entries);
        Assert.True(alphabet.DecodableCount > 1, $"{language} decodes only the word separator");
    }

    [Fact]
    public void AlphabetNamesMatchTheLanguageEnum()
    {
        foreach (Language language in Enum.GetValues<Language>())
            Assert.Equal(language.ToString(), Alphabets.For(language).Name);
    }
}
