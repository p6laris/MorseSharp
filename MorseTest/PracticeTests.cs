namespace MorseTest;

public class PracticeTests
{
    private static string Encode(string text) =>
        Morse.GetConverter().ForLanguage(Language.English).ToMorse(text).Encode();

    [Fact]
    public void TheKochOrderIsFortyDistinctCharacters()
    {
        ReadOnlySpan<char> order = Koch.Order;

        Assert.Equal(40, order.Length);
        Assert.Equal(order.Length, Koch.MaxLevel);
        Assert.Equal(order.Length, new HashSet<char>(order.ToArray()).Count);
        Assert.Equal('K', order[0]);
        Assert.Equal('M', order[1]);
    }

    [Fact]
    public void EveryKochCharacterCanBeSent()
    {
        // A lesson the library cannot key would be useless, so this is the property that matters.
        foreach (char character in Koch.Order)
            Assert.NotEmpty(Encode(character.ToString()));
    }

    [Theory]
    [InlineData(1, 1, 5)]
    [InlineData(2, 10, 5)]
    [InlineData(40, 4, 3)]
    public void ALessonHasTheShapeItWasAskedFor(int level, int groups, int groupSize)
    {
        string lesson = Koch.Generate(level, groups, groupSize, new Random(1));

        Assert.Equal(Koch.LengthFor(groups, groupSize), lesson.Length);

        string[] parts = lesson.Split(' ');
        Assert.Equal(groups, parts.Length);
        Assert.All(parts, part => Assert.Equal(groupSize, part.Length));
    }

    [Fact]
    public void ALessonOnlyUsesTheCharactersUnlockedSoFar()
    {
        for (int level = 1; level <= Koch.MaxLevel; level++)
        {
            string lesson = Koch.Generate(level, groups: 20, random: new Random(level));
            HashSet<char> allowed = new(Koch.Order[..level].ToArray());

            foreach (char character in lesson.Replace(" ", ""))
                Assert.Contains(character, allowed);
        }
    }

    [Fact]
    public void ALessonUsesEveryCharacterUnlockedSoFar()
    {
        // Enough groups that leaving one out would be a real bug rather than bad luck.
        string lesson = Koch.Generate(level: 5, groups: 200, random: new Random(7));

        foreach (char character in Koch.Order[..5])
            Assert.Contains(character, lesson);
    }

    [Fact]
    public void ALessonCanBeSent()
    {
        string lesson = Koch.Generate(level: Koch.MaxLevel, groups: 20, random: new Random(3));
        Assert.NotEmpty(Encode(lesson));
    }

    [Fact]
    public void TheSameSeedGivesTheSameLesson()
    {
        Assert.Equal(Koch.Generate(10, 5, random: new Random(42)), Koch.Generate(10, 5, random: new Random(42)));
        Assert.NotEqual(Koch.Generate(10, 5, random: new Random(42)), Koch.Generate(10, 5, random: new Random(43)));
    }

    [Fact]
    public void TheBufferOverloadWritesTheSameThing()
    {
        Span<char> buffer = stackalloc char[Koch.LengthFor(6)];
        int written = Koch.Generate(buffer, level: 8, groups: 6, random: new Random(11));

        Assert.Equal(buffer.Length, written);
        Assert.Equal(Koch.Generate(8, 6, random: new Random(11)), new string(buffer));
    }

    [Theory]
    [InlineData(0, 5, 5)]
    [InlineData(41, 5, 5)]
    [InlineData(5, 0, 5)]
    [InlineData(5, 5, 0)]
    public void ABadLessonRequestIsRejected(int level, int groups, int groupSize)
        => Assert.Throws<ArgumentOutOfRangeException>(() => Koch.Generate(level, groups, groupSize));

    [Fact]
    public void ATooSmallBufferIsRejected()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Span<char> tooSmall = stackalloc char[4];
            Koch.Generate(tooSmall, level: 2, groups: 5);
        });
    }

    [Fact]
    public void APerfectCopyScoresEverything()
    {
        KochScore score = Koch.Score("KMRS MKRS", "KMRS MKRS");

        Assert.Equal(8, score.Total);
        Assert.Equal(8, score.Correct);
        Assert.Equal(1.0, score.Accuracy);
        Assert.True(score.ClearsThreshold);
    }

    [Fact]
    public void ScoringIgnoresSpacingAndCase()
    {
        Assert.Equal(new KochScore(8, 8), Koch.Score("KMRS MKRS", "kmrsmkrs"));
        Assert.Equal(new KochScore(8, 8), Koch.Score("KMRSMKRS", "KM RS MK RS"));
    }

    [Fact]
    public void AWrongCharacterCostsOne()
    {
        KochScore score = Koch.Score("KMRS", "KMRU");

        Assert.Equal(new KochScore(3, 4), score);
        Assert.Equal(0.75, score.Accuracy);
        Assert.False(score.ClearsThreshold);
    }

    [Fact]
    public void ACopyThatStopsShortScoresTheRest()
    {
        Assert.Equal(new KochScore(2, 4), Koch.Score("KMRS", "KM"));
        Assert.Equal(new KochScore(0, 4), Koch.Score("KMRS", ""));
        Assert.Equal(0, Koch.Score("", "").Accuracy);
    }

    [Fact]
    public void NinetyPerCentIsTheBar()
    {
        Assert.True(new KochScore(9, 10).ClearsThreshold);
        Assert.False(new KochScore(89, 100).ClearsThreshold);
        Assert.True(new KochScore(90, 100).ClearsThreshold);
    }

    [Fact]
    public void ACallsignLooksLikeOne()
    {
        Random random = new(5);
        for (int i = 0; i < 500; i++)
        {
            string call = Callsign.Next(random);

            Assert.InRange(call.Length, 3, Callsign.MaxLength);
            Assert.All(call, character => Assert.True(char.IsAsciiLetterUpper(character) || char.IsAsciiDigit(character), call));
            Assert.Contains(call, char.IsAsciiDigit);
            Assert.True(char.IsAsciiLetterUpper(call[^1]), call);
        }
    }

    [Fact]
    public void ACallsignCanBeSent()
    {
        Random random = new(6);
        for (int i = 0; i < 200; i++)
            Assert.NotEmpty(Encode(Callsign.Next(random)));
    }

    [Fact]
    public void TheSameSeedGivesTheSameCallsign()
    {
        Assert.Equal(Callsign.Next(new Random(9)), Callsign.Next(new Random(9)));

        Span<char> buffer = stackalloc char[Callsign.MaxLength];
        int written = Callsign.Next(buffer, new Random(9));
        Assert.Equal(Callsign.Next(new Random(9)), new string(buffer[..written]));
    }

    [Fact]
    public void ATooSmallCallsignBufferIsRejected()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Span<char> tooSmall = stackalloc char[Callsign.MaxLength - 1];
            Callsign.Next(tooSmall);
        });
    }

    [Fact]
    public void AContactRunsInTheRightOrder()
    {
        string[] qso = Qso.Generate(new Random(21));

        Assert.Equal(5, qso.Length);
        Assert.StartsWith("CQ CQ CQ DE ", qso[0]);
        Assert.EndsWith("<SK>", qso[^1]);

        // The two stations call each other by name throughout.
        string caller = qso[0].Split(' ')[4];
        string answerer = qso[1].Split(' ')[2];

        Assert.NotEqual(caller, answerer);
        Assert.All(qso, transmission => Assert.Contains(caller, transmission));
        Assert.All(qso[1..], transmission => Assert.Contains(answerer, transmission));
    }

    [Fact]
    public void EveryTransmissionCanBeSent()
    {
        Random random = new(22);
        for (int i = 0; i < 50; i++)
        {
            foreach (string transmission in Qso.Generate(random))
                Assert.NotEmpty(Encode(transmission));
        }
    }

    [Fact]
    public void TheProsignIsKeyedAsOneSignal()
    {
        string[] qso = Qso.Generate(new Random(23));

        // <SK> is a single unbroken signal, so it must not read back as S then K.
        Assert.Contains("...-.-", Encode(qso[^1]));
    }

    [Fact]
    public void TheSameSeedGivesTheSameContact()
        => Assert.Equal(Qso.Generate(new Random(24)), Qso.Generate(new Random(24)));
}
