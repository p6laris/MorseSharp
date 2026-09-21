using MorseSharp;

try
{
    // Encoding
    string morse = Morse.GetConverter()
        .ForLanguage(Language.English)
        .ToMorse("Hi")
        .Encode();
    Console.WriteLine($"Hi -> {morse}");

    // Decoding
    string text = Morse.GetConverter()
        .ForLanguage(Language.English)
        .Decode(".... ..");
    Console.WriteLine($".... .. -> {text}");

    // Audio: write a WAV file straight to disk (no intermediate array)
    using (FileStream file = File.Create("hi.wav"))
    {
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse("Hi")
            .ToAudio()
            .SetAudioOptions(charSpeed: 25, wordSpeed: 15, frequency: 700)
            .WriteTo(file);
    }
    Console.WriteLine("Wrote hi.wav");

    // Audio: fill your own buffer without allocating
    var audio = Morse.GetConverter()
        .ForLanguage(Language.Kurdish)
        .ToMorse("سڵاو")
        .ToAudio()
        .SetAudioOptions();
    byte[] buffer = new byte[audio.GetByteCount()];
    int written = audio.GetBytes(buffer);
    Console.WriteLine($"Kurdish audio: {written} bytes");

    // Light blinking (Ctrl+C cancels)
    using CancellationTokenSource cts = new();
    Console.CancelKeyPress += (_, e) => { e.Cancel = true; cts.Cancel(); };

    await Morse.GetConverter()
        .ForLanguage(Language.English)
        .ToLight(".... ..")
        .SetBlinkerOptions(25, 25)
        .DoBlinks(on => Console.BackgroundColor = on ? ConsoleColor.White : ConsoleColor.Black, cts.Token);
    Console.ResetColor();
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
