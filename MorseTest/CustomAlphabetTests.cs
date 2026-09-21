namespace MorseTest;

public class CustomAlphabetTests
{
    [Fact]
    public void BuildsAndUsesACustomAlphabet()
    {
        MorseAlphabet klingon = new MorseAlphabetBuilder("Klingon")
            .Add('a', ".-")
            .Add('b', "-...")
            .Build();

        var conv = Morse.GetConverter().ForAlphabet(klingon);

        Assert.Equal("Klingon", klingon.Name);
        Assert.Equal(".- -...", conv.ToMorse("ab").Encode());

        // Decoding returns characters exactly as registered, and these were registered lower case.
        Assert.Equal("ab", conv.Decode(".- -..."));
    }

    [Fact]
    public void CustomAlphabetRejectsCharactersItDoesNotHave()
    {
        MorseAlphabet tiny = new MorseAlphabetBuilder("Tiny").Add('a', ".-").Build();

        var ex = Assert.Throws<CharacterNotPresentedException>(() =>
            Morse.GetConverter().ForAlphabet(tiny).ToMorse("az"));

        Assert.Equal('z', ex.Character);
        Assert.Equal("Tiny", ex.AlphabetName);
    }

    [Fact]
    public void CustomAlphabetNameReachesDecodeErrors()
    {
        MorseAlphabet tiny = new MorseAlphabetBuilder("Tiny").Add('a', ".-").Build();

        var ex = Assert.Throws<SequenceNotFoundException>(() =>
            Morse.GetConverter().ForAlphabet(tiny).Decode("---"));

        Assert.Equal("Tiny", ex.AlphabetName);
    }

    [Fact]
    public void ExtendsABuiltInLanguage()
    {
        MorseAlphabet extended = MorseAlphabetBuilder.From(Language.Deutsch)
            .Add('Ə', "..--.")
            .Build();

        var conv = Morse.GetConverter().ForAlphabet(extended);

        Assert.Equal("Deutsch", extended.Name);
        Assert.Equal("..--.", conv.ToMorse("Ə").Encode());
        Assert.Equal("--. .-. --- ......", conv.ToMorse("groß").Encode());   // the original entries survive
    }

    [Fact]
    public void RemovesACharacterFromABuiltInLanguage()
    {
        MorseAlphabet trimmed = MorseAlphabetBuilder.From(Language.Deutsch)
            .Remove('$')
            .Build();

        Assert.Throws<CharacterNotPresentedException>(() =>
            Morse.GetConverter().ForAlphabet(trimmed).ToMorse("$"));

        // The untouched language still has it.
        Assert.Equal("...-..-", Morse.GetConverter().ForLanguage(Language.Deutsch).ToMorse("$").Encode());
    }

    [Fact]
    public void RemovingAPrimaryPromotesItsAlias()
    {
        // 'A' owns ".-" and 'B' rides along as an alias. Removing 'A' should let 'B' own the pattern.
        MorseAlphabet promoted = new MorseAlphabetBuilder("Promote")
            .Add('A', ".-")
            .AddAlias('B', ".-")
            .Remove('A')
            .Build();

        Assert.Equal("B", Morse.GetConverter().ForAlphabet(promoted).Decode(".-"));
    }

    [Fact]
    public void AliasEncodesButDoesNotOwnThePattern()
    {
        MorseAlphabet alphabet = new MorseAlphabetBuilder("Aliased")
            .Add('A', ".-")
            .AddAlias('B', ".-")
            .Build();

        var conv = Morse.GetConverter().ForAlphabet(alphabet);

        Assert.Equal(".-", conv.ToMorse("A").Encode());
        Assert.Equal(".-", conv.ToMorse("B").Encode());
        Assert.Equal("A", conv.Decode(".-"));
    }

    [Fact]
    public void AliasOrderDoesNotMatter()
    {
        // The alias is declared first, but the primary must still win the pattern.
        MorseAlphabet alphabet = new MorseAlphabetBuilder("Ordering")
            .AddAlias('B', ".-")
            .Add('A', ".-")
            .Build();

        Assert.Equal("A", Morse.GetConverter().ForAlphabet(alphabet).Decode(".-"));
    }

    [Fact]
    public void RemoveIsSilentForAnAbsentCharacter()
    {
        MorseAlphabet alphabet = new MorseAlphabetBuilder("Tiny").Add('a', ".-").Remove('z').Build();
        Assert.Equal(".-", Morse.GetConverter().ForAlphabet(alphabet).ToMorse("a").Encode());
    }

    [Fact]
    public void BuilderCanBeReusedWithoutAffectingEarlierAlphabets()
    {
        MorseAlphabetBuilder builder = new MorseAlphabetBuilder("Reused").Add('a', ".-");
        MorseAlphabet first = builder.Build();

        builder.Add('b', "-...");
        MorseAlphabet second = builder.Build();

        Assert.Throws<CharacterNotPresentedException>(() =>
            Morse.GetConverter().ForAlphabet(first).ToMorse("b"));
        Assert.Equal("-...", Morse.GetConverter().ForAlphabet(second).ToMorse("b").Encode());
    }

    [Fact]
    public void CaseVariantsAreAcceptedForCustomEntries()
    {
        MorseAlphabet alphabet = new MorseAlphabetBuilder("Cased").Add('a', ".-").Build();
        var conv = Morse.GetConverter().ForAlphabet(alphabet);

        Assert.Equal(".-", conv.ToMorse("a").Encode());
        Assert.Equal(".-", conv.ToMorse("A").Encode());
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void BuilderRejectsABadName(string? name)
    {
        Assert.ThrowsAny<ArgumentException>(() => new MorseAlphabetBuilder(name!));
    }

    [Theory]
    [InlineData(".........")]   // nine symbols, one too many
    [InlineData(".x.")]
    [InlineData("/")]
    public void BuilderRejectsABadPatternAtTheCallSite(string pattern)
    {
        MorseAlphabetBuilder builder = new("Bad");
        Assert.Throws<ArgumentException>(() => builder.Add('a', pattern));
    }

    [Fact]
    public void BuilderRejectsAnEmptyPattern()
    {
        MorseAlphabetBuilder builder = new("Bad");
        Assert.Throws<ArgumentException>(() => builder.Add('a', ""));
        Assert.Throws<ArgumentNullException>(() => builder.Add('a', null!));
    }

    [Fact]
    public void EmptyAlphabetIsRejected()
    {
        Assert.Throws<InvalidOperationException>(() => new MorseAlphabetBuilder("Empty").Build());
    }

    [Fact]
    public void OneCharacterMappedToTwoPatternsIsRejected()
    {
        MorseAlphabetBuilder builder = new MorseAlphabetBuilder("Conflict")
            .Add('a', ".-")
            .Add('a', "-...");

        Assert.Throws<InvalidOperationException>(() => builder.Build());
    }

    [Fact]
    public void ForAlphabetRejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Morse.GetConverter().ForAlphabet(null!));
    }

    [Fact]
    public void FromRejectsNull()
    {
        Assert.Throws<ArgumentNullException>(() => MorseAlphabetBuilder.From((MorseAlphabet)null!));
    }

    [Fact]
    public void LargeAlphabetExceedsTheOldFixedTable()
    {
        // The table used to be a hard-coded 256 slots. 300 non-ASCII characters would have thrown
        // "hash table is full"; now the table is sized to the alphabet.
        const int count = 300;
        MorseAlphabetBuilder builder = new("Large");

        List<(char Char, string Pattern)> expected = [];
        int index = 0;
        for (int length = 1; length <= MorseAlphabet.MaxSymbols && expected.Count < count; length++)
        {
            for (int bits = 0; bits < 1 << length && expected.Count < count; bits++)
            {
                string pattern = string.Create(length, bits, (span, b) =>
                {
                    for (int i = 0; i < span.Length; i++)
                        span[i] = ((b >> (span.Length - 1 - i)) & 1) != 0 ? '-' : '.';
                });

                char ch = (char)('一' + index++);   // CJK, non-ASCII and caseless
                builder.Add(ch, pattern);
                expected.Add((ch, pattern));
            }
        }

        MorseAlphabet large = builder.Build();
        var conv = Morse.GetConverter().ForAlphabet(large);

        Assert.Equal(count, expected.Count);
        Assert.True(large.HashedSlots >= count, $"table held {large.HashedSlots} slots for {count} characters");

        foreach ((char ch, string pattern) in expected)
        {
            Assert.Equal(pattern, conv.ToMorse(ch.ToString()).Encode());
            Assert.Equal(ch, Assert.Single(conv.Decode(pattern)));
        }
    }

    [Fact]
    public void PureAsciiAlphabetAllocatesNoHashTable()
    {
        MorseAlphabet ascii = new MorseAlphabetBuilder("Ascii")
            .Add('a', ".-")
            .Add('b', "-...")
            .Build();

        Assert.Equal(0, ascii.HashedSlots);
        Assert.Equal(".- -...", Morse.GetConverter().ForAlphabet(ascii).ToMorse("ab").Encode());
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 4)]
    [InlineData(2, 4)]
    [InlineData(3, 8)]
    [InlineData(16, 32)]
    [InlineData(76, 256)]
    public void HashTableSizeKeepsLoadFactorAtOrBelowOneHalf(int slots, int expected)
    {
        Assert.Equal(expected, MorseAlphabet.ComputeHashedSize(slots));
    }

    [Fact]
    public void BuiltInAlphabetsAreSizedToTheirContent()
    {
        // The whole point of the resize: languages no longer all pay for 256 slots.
        Assert.True(Alphabets.English.HashedSlots < 256, $"English used {Alphabets.English.HashedSlots} slots");
        Assert.True(Alphabets.Deutsch.HashedSlots < 256, $"Deutsch used {Alphabets.Deutsch.HashedSlots} slots");

        foreach (Language language in Enum.GetValues<Language>())
        {
            MorseAlphabet alphabet = Alphabets.For(language);
            Assert.InRange(alphabet.MaxProbeLength, 0, 4);
        }
    }
}
