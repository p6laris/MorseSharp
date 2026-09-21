namespace MorseTest;

public class LightTests
{
    [Fact]
    public async Task SequenceAlternatesStartsOnAndEndsOff()
    {
        List<bool> events = [];
        await Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse("Hi")
            .ToLight()
            .SetBlinkerOptions(200, 200)
            .DoBlinks(events.Add);

        // H = 4 dots (3 element gaps), character gap, I = 2 dots (1 element gap), trailing character gap.
        Assert.Equal(12, events.Count);
        Assert.Equal(6, events.Count(on => on));
        Assert.True(events[0]);
        Assert.False(events[^1]);
        for (int i = 1; i < events.Count; i++)
            Assert.NotEqual(events[i - 1], events[i]);
    }

    [Fact]
    public async Task ManualMorseProducesTheSameEvents()
    {
        List<bool> fromText = [];
        List<bool> fromMorse = [];

        await Morse.GetConverter().ForLanguage(Language.English).ToMorse("SOS").ToLight().SetBlinkerOptions(200, 200).DoBlinks(fromText.Add);
        await Morse.GetConverter().ForLanguage(Language.English).ToLight("... --- ...").SetBlinkerOptions(200, 200).DoBlinks(fromMorse.Add);

        Assert.Equal(fromText, fromMorse);
    }

    [Fact]
    public async Task CancellationSwitchesTheLightOff()
    {
        List<bool> events = [];
        using CancellationTokenSource cts = new();

        Task task = Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse("SOS SOS SOS")
            .ToLight()
            .SetBlinkerOptions(5, 5)
            .DoBlinks(on =>
            {
                events.Add(on);
                if (events.Count == 1)
                    cts.Cancel();
            }, cts.Token);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => task);
        Assert.True(events[0]);
        Assert.False(events[^1]);
    }

    [Fact]
    public async Task PlaybackTakesRoughlyTheExpectedTime()
    {
        // "EEEEE" at 100 wpm: 5 dots of 12 ms + 4 character gaps of 36 ms + a trailing gap of 36 ms = 240 ms.
        Stopwatch stopwatch = Stopwatch.StartNew();
        await Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse("EEEEE")
            .ToLight()
            .SetBlinkerOptions(100, 100)
            .DoBlinks(_ => { });
        stopwatch.Stop();

        Assert.InRange(stopwatch.Elapsed.TotalMilliseconds, 200, 1500);
    }

    [Fact]
    public async Task NullActionThrows()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            Morse.GetConverter().ForLanguage(Language.English).ToLight("...").SetBlinkerOptions().DoBlinks(null!));
    }
}
