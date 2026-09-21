namespace MorseTest;

public class MorseAudioConverterTest
{
    // Latin
    [Fact]
    public void ConvertToAudioEnglish()
    {
        byte[] wav = Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse("Hello Morse")
            .ToAudio()
            .SetAudioOptions(25, 25, 600)
            .GetBytes();

        Assert.True(wav.Length > 44);
    }

    // Non-Latin
    [Fact]
    public void ConvertToAudioKurdish()
    {
        byte[] wav = Morse.GetConverter()
            .ForLanguage(Language.Kurdish)
            .ToMorse("کووی")
            .ToAudio()
            .SetAudioOptions(25, 25, 600)
            .GetBytes();

        Assert.True(wav.Length > 44);
    }

    [Fact]
    public void ConvertToAudioSmallerWordSpeed()
    {
        Assert.Throws<SmallerCharSpeedException>(() =>
            Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToMorse("Hello Morse")
                .ToAudio()
                .SetAudioOptions(20, 25, 600)
                .GetBytes());
    }

    [Fact]
    public void ConvertToAudioWithoutConversion()
    {
        byte[] wav = Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToAudio(".... ..")
            .SetAudioOptions(25, 25, 600)
            .GetBytes();

        Assert.True(wav.Length > 44);
    }

    [Fact]
    public void ConvertToAudioWithoutConversionSmallerCharSpeed()
    {
        Assert.Throws<SmallerCharSpeedException>(() =>
            Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToAudio(".... ..")
                .SetAudioOptions(20, 25, 600)
                .GetBytes());
    }

    [Fact]
    public void ConvertToAudioWithoutConversionNullMorse()
    {
        Assert.Throws<ArgumentNullException>(() =>
            Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToAudio(null!)
                .SetAudioOptions(25, 25, 600)
                .GetBytes());
    }

    // Light blinker
    [Fact]
    public async Task ConvertToLight()
    {
        List<bool> blinkValues = [];
        await Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse("Hi")
            .ToLight()
            .SetBlinkerOptions(200, 200)
            .DoBlinks(blinkValues.Add);

        Assert.True(blinkValues.Count > 0);
    }

    [Fact]
    public async Task ConvertToLightWithNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToMorse(null!)
                .ToLight()
                .SetBlinkerOptions(25, 25)
                .DoBlinks(_ => { }));
    }

    [Fact]
    public async Task ConvertToLightWithSmallerCharSpeed()
    {
        await Assert.ThrowsAsync<SmallerCharSpeedException>(async () =>
            await Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToMorse("Hi")
                .ToLight()
                .SetBlinkerOptions(20, 25)
                .DoBlinks(_ => { }));
    }

    [Fact]
    public async Task ConvertToLightWithoutConversionNull()
    {
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToLight(null!)
                .SetBlinkerOptions(25, 25)
                .DoBlinks(_ => { }));
    }

    [Fact]
    public async Task ConvertToLightWithoutConversionSmallerCharSpeed()
    {
        await Assert.ThrowsAsync<SmallerCharSpeedException>(async () =>
            await Morse.GetConverter()
                .ForLanguage(Language.English)
                .ToLight(".... ..")
                .SetBlinkerOptions(20, 25)
                .DoBlinks(_ => { }));
    }
}
