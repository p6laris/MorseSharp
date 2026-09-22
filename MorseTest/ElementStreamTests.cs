using MorseSharp.Core;

namespace MorseTest;

public class ElementStreamTests
{
    private static MorseElementSequence Elements(string text, int charSpeed = 20, int wordSpeed = 20) =>
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse(text)
            .ToLight()
            .SetBlinkerOptions(charSpeed, wordSpeed)
            .GetElements();

    private static List<MorseElement> ToList(MorseElementSequence sequence)
    {
        List<MorseElement> list = [];
        foreach (MorseElement element in sequence)
            list.Add(element);

        return list;
    }

    [Fact]
    public void TheElementsSpellOutTheEncodedMorse()
    {
        StringBuilder rebuilt = new();
        foreach (MorseElement element in Elements("CQ DE"))
        {
            switch (element.Kind)
            {
                case MorseElementKind.Dot: rebuilt.Append('.'); break;
                case MorseElementKind.Dash: rebuilt.Append('-'); break;
                case MorseElementKind.CharGap: rebuilt.Append(' '); break;
                case MorseElementKind.WordGap: rebuilt.Append(" / "); break;
            }
        }

        // A word gap stands in for the character gap on either side of it, so it writes the whole " / " separator.
        // The walk also ends on a character gap, which writes a trailing space the encoder does not.
        Assert.Equal("-.-. --.- / -.. .", rebuilt.ToString().TrimEnd());
        Assert.Equal("-.-. --.- / -.. .", Morse.GetConverter().ForLanguage(Language.English).ToMorse("CQ DE").Encode());
    }

    [Fact]
    public void AMorseStringGivesTheSameElementsAsTheText()
    {
        List<MorseElement> fromText = ToList(Elements("SOS SOS"));
        List<MorseElement> fromMorse = ToList(Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToLight("... --- ... / ... --- ...")
            .SetBlinkerOptions(20, 20)
            .GetElements());

        Assert.Equal(fromText, fromMorse);
    }

    [Fact]
    public async Task KeyDownMatchesWhatTheBlinkerReports()
    {
        List<bool> blinks = [];
        await Morse.GetConverter().ForLanguage(Language.English).ToMorse("Hello World").ToLight()
            .SetBlinkerOptions(200, 200).DoBlinks(blinks.Add);

        Assert.Equal(blinks, ToList(Elements("Hello World", 200, 200)).Select(element => element.KeyDown));
    }

    [Fact]
    public void EveryKindCarriesItsFarnsworthDuration()
    {
        // At 20 wpm a dit is 1.2 / 20 = 60 ms, and with equal speeds the gaps collapse to the standard 1, 3 and 7 dits.
        Dictionary<MorseElementKind, TimeSpan> byKind = [];
        foreach (MorseElement element in Elements("A B"))
            byKind[element.Kind] = element.Duration;

        Assert.Equal(TimeSpan.FromMilliseconds(60), byKind[MorseElementKind.Dot]);
        Assert.Equal(TimeSpan.FromMilliseconds(180), byKind[MorseElementKind.Dash]);
        Assert.Equal(TimeSpan.FromMilliseconds(60), byKind[MorseElementKind.ElementGap]);
        Assert.Equal(TimeSpan.FromMilliseconds(180), byKind[MorseElementKind.CharGap]);
        Assert.Equal(TimeSpan.FromMilliseconds(420), byKind[MorseElementKind.WordGap]);
    }

    [Fact]
    public void FarnsworthStretchesOnlyTheGaps()
    {
        Dictionary<MorseElementKind, TimeSpan> stretched = [];
        foreach (MorseElement element in Elements("A B", charSpeed: 20, wordSpeed: 10))
            stretched[element.Kind] = element.Duration;

        Assert.Equal(TimeSpan.FromMilliseconds(60), stretched[MorseElementKind.Dot]);
        Assert.True(stretched[MorseElementKind.CharGap] > TimeSpan.FromMilliseconds(180));
    }

    [Fact]
    public void DurationIsTheSumOfTheElements()
    {
        MorseElementSequence sequence = Elements("PARIS");
        Assert.Equal(ToList(sequence).Aggregate(TimeSpan.Zero, (total, element) => total + element.Duration), sequence.Duration);
    }

    [Fact]
    public void ASequenceSurvivesTheNextChainOnTheSameThread()
    {
        MorseElementSequence sequence = Elements("SOS");
        List<MorseElement> before = ToList(sequence);

        // Chain state is thread-static, so this replaces what produced the sequence above.
        Morse.GetConverter().ForLanguage(Language.Deutsch).ToMorse("ABC").ToLight().SetBlinkerOptions(5, 5).GetElements();

        Assert.Equal(before, ToList(sequence));
    }

    [Fact]
    public void EnumeratingAllocatesNothing()
    {
        MorseElementSequence sequence = Elements("CQ DE MORSE");

        int expected = 0;
        foreach (MorseElement _ in sequence)
            expected++;

        long before = GC.GetAllocatedBytesForCurrentThread();
        int count = 0;
        foreach (MorseElement _ in sequence)
            count++;
        long after = GC.GetAllocatedBytesForCurrentThread();

        Assert.Equal(expected, count);
        Assert.Equal(0, after - before);
    }

    [Fact]
    public void TheSequenceAlwaysEndsKeyUp()
    {
        List<MorseElement> elements = ToList(Morse.GetConverter()
            .ForLanguage(Language.English).ToLight(".").SetBlinkerOptions(200, 200).GetElements());

        Assert.Equal(MorseElementKind.Dot, elements[0].Kind);
        Assert.Equal(MorseElementKind.CharGap, elements[^1].Kind);
        Assert.False(elements[^1].KeyDown);
    }

    [Fact]
    public async Task PlayAsyncYieldsExactlyWhatGetElementsDoes()
    {
        List<MorseElement> played = [];
        await foreach (MorseElement element in Morse.GetConverter()
            .ForLanguage(Language.English).ToMorse("Hi there").ToLight().SetBlinkerOptions(200, 200).PlayAsync())
        {
            played.Add(element);
        }

        Assert.Equal(ToList(Elements("Hi there", 200, 200)), played);
    }

    [Fact]
    public async Task PlayAsyncCanBeStoppedEarly()
    {
        // At 5 wpm the whole message would take about twenty seconds, so finishing quickly proves it really stopped.
        Stopwatch stopwatch = Stopwatch.StartNew();
        int seen = 0;

        await foreach (MorseElement _ in Morse.GetConverter()
            .ForLanguage(Language.English).ToMorse("SOS SOS SOS").ToLight().SetBlinkerOptions(5, 5).PlayAsync())
        {
            if (++seen == 3)
                break;
        }

        stopwatch.Stop();
        Assert.Equal(3, seen);
        Assert.True(stopwatch.Elapsed < TimeSpan.FromSeconds(5), $"took {stopwatch.Elapsed}");
    }

    [Fact]
    public async Task PlayAsyncHonoursTheTokenGivenToTheMethod()
    {
        using CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (MorseElement _ in Morse.GetConverter()
                .ForLanguage(Language.English).ToMorse("SOS SOS SOS").ToLight().SetBlinkerOptions(5, 5).PlayAsync(cts.Token))
            {
            }
        });
    }

    [Fact]
    public async Task PlayAsyncHonoursTheTokenGivenToTheForeach()
    {
        using CancellationTokenSource cts = new();
        cts.CancelAfter(TimeSpan.FromMilliseconds(50));

        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () =>
        {
            await foreach (MorseElement _ in Morse.GetConverter()
                .ForLanguage(Language.English).ToMorse("SOS SOS SOS").ToLight()
                .SetBlinkerOptions(5, 5).PlayAsync().WithCancellation(cts.Token))
            {
            }
        });
    }

    [Fact]
    public async Task PlayAsyncTakesRoughlyTheExpectedTime()
    {
        MorseElementSequence sequence = Elements("EEEEE", 100, 100);

        Stopwatch stopwatch = Stopwatch.StartNew();
        await foreach (MorseElement _ in Morse.GetConverter()
            .ForLanguage(Language.English).ToMorse("EEEEE").ToLight().SetBlinkerOptions(100, 100).PlayAsync())
        {
        }
        stopwatch.Stop();

        Assert.InRange(stopwatch.Elapsed, sequence.Duration - TimeSpan.FromMilliseconds(60), TimeSpan.FromSeconds(2));
    }

    [Fact]
    public async Task OneTimerCoversTheWholeStream()
    {
        // The point of hand-rolling the enumerator: a long message must not cost a timer per element.
        const string Message = "CQ CQ DE MORSE MORSE K";

        ManualTime time = new();
        List<MorseElement> played = [];

        MorseElementStream stream = new(Elements(Message), time, CancellationToken.None);
        await using IAsyncEnumerator<MorseElement> enumerator = stream.GetAsyncEnumerator();

        while (true)
        {
            ValueTask<bool> next = enumerator.MoveNextAsync();
            if (!next.IsCompleted)
                time.FireNextTimer();

            if (!await next)
                break;

            played.Add(enumerator.Current);
        }

        Assert.Equal(ToList(Elements(Message)), played);
        Assert.True(played.Count > 90, $"expected a long message, got {played.Count} elements");

        // One timer, re-armed once per element, however long the message runs.
        Assert.Equal(1, time.TimersCreated);
        Assert.Equal(played.Count, time.Waits);
    }

    /// <summary>A clock that only moves when the test says so, counting how many timers the stream asks for.</summary>
    private sealed class ManualTime : TimeProvider
    {
        private readonly List<ManualTimer> _timers = [];
        private long _now;

        public int TimersCreated { get; private set; }

        public int Waits { get; private set; }

        public override long TimestampFrequency => TimeSpan.TicksPerSecond;

        public override long GetTimestamp() => _now;

        public override ITimer CreateTimer(TimerCallback callback, object? state, TimeSpan dueTime, TimeSpan period)
        {
            TimersCreated++;
            ManualTimer timer = new(this, callback, state);
            _timers.Add(timer);
            timer.Change(dueTime, period);
            return timer;
        }

        /// <summary>Jumps the clock to the earliest armed timer and runs it.</summary>
        public void FireNextTimer()
        {
            ManualTimer? due = null;
            foreach (ManualTimer timer in _timers)
            {
                if (timer.DueAt < (due?.DueAt ?? long.MaxValue))
                    due = timer;
            }

            Assert.NotNull(due);
            Waits++;
            _now = due.DueAt;
            due.Fire();
        }

        internal long Now => _now;

        private sealed class ManualTimer(ManualTime time, TimerCallback callback, object? state) : ITimer
        {
            public long DueAt { get; private set; } = long.MaxValue;

            public bool Change(TimeSpan dueTime, TimeSpan period)
            {
                DueAt = dueTime == Timeout.InfiniteTimeSpan ? long.MaxValue : time.Now + dueTime.Ticks;
                return true;
            }

            public void Fire()
            {
                DueAt = long.MaxValue;
                callback(state);
            }

            public void Dispose() => DueAt = long.MaxValue;

            public ValueTask DisposeAsync()
            {
                Dispose();
                return ValueTask.CompletedTask;
            }
        }
    }
}
