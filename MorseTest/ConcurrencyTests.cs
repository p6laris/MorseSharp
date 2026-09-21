namespace MorseTest;

public class ConcurrencyTests
{
    [Fact]
    public async Task ChainsOnDifferentThreadsDoNotInterfere()
    {
        const string englishText = "The quick brown fox";
        const string kurdishText = "کۆژین و ڤیان";

        string expectedEnglish = Morse.GetConverter().ForLanguage(Language.English).ToMorse(englishText).Encode();
        string expectedKurdish = Morse.GetConverter().ForLanguage(Language.Kurdish).ToMorse(kurdishText).Encode();

        Task[] workers = new Task[Environment.ProcessorCount * 2];
        for (int i = 0; i < workers.Length; i++)
        {
            bool english = i % 2 == 0;
            workers[i] = Task.Run(() =>
            {
                for (int round = 0; round < 200; round++)
                {
                    if (english)
                        Assert.Equal(expectedEnglish, Morse.GetConverter().ForLanguage(Language.English).ToMorse(englishText).Encode());
                    else
                        Assert.Equal(expectedKurdish, Morse.GetConverter().ForLanguage(Language.Kurdish).ToMorse(kurdishText).Encode());
                }
            });
        }

        await Task.WhenAll(workers);
    }

    [Fact]
    public async Task AudioIsIdenticalAcrossThreads()
    {
        byte[] expected = Morse.GetConverter().ForLanguage(Language.English).ToMorse("SOS").ToAudio().SetAudioOptions(25, 20, 700).GetBytes();

        Task<byte[]>[] workers = new Task<byte[]>[8];
        for (int i = 0; i < workers.Length; i++)
        {
            workers[i] = Task.Run(() =>
                Morse.GetConverter().ForLanguage(Language.English).ToMorse("SOS").ToAudio().SetAudioOptions(25, 20, 700).GetBytes());
        }

        foreach (byte[] actual in await Task.WhenAll(workers))
            Assert.Equal(expected, actual);
    }

    [Fact]
    public void SwitchingLanguageMidChainUsesTheLatestOne()
    {
        var conv = Morse.GetConverter().ForLanguage(Language.English);
        Assert.Equal(".-", conv.ToMorse("A").Encode());

        conv = Morse.GetConverter().ForLanguage(Language.Kurdish);
        Assert.Equal(".-", conv.ToMorse("ا").Encode());
    }
}
