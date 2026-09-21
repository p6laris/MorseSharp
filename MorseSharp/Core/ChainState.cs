namespace MorseSharp.Core;

/// <summary>
/// The state of the fluent chain currently being built on this thread: the alphabet, what is being converted and
/// the timing options. One instance per thread is created lazily and reused for every chain on that thread.
/// </summary>
internal sealed class ChainState
{
    /// <summary>The alphabet chosen by <c>ForLanguage</c>.</summary>
    public MorseAlphabet Alphabet = Alphabets.English;

    /// <summary>The text passed to <c>ToMorse</c>, or <c>null</c> when a Morse string was supplied directly.</summary>
    public string? Text;

    /// <summary>The Morse string passed to <c>ToAudio</c>/<c>ToLight</c>, or <c>null</c> when text was supplied.</summary>
    public string? Morse;

    /// <summary>Length of the string <c>Encode</c> will produce, computed while validating the text.</summary>
    public int EncodedLength;

    /// <summary>Character speed in words per minute.</summary>
    public int CharSpeed = 25;

    /// <summary>Word speed in words per minute.</summary>
    public int WordSpeed = 25;

    /// <summary>Tone frequency in hertz.</summary>
    public double Frequency = 700;

    /// <summary>The element source for the current chain.</summary>
    /// <exception cref="InvalidOperationException">Thrown when no input has been supplied yet.</exception>
    public ElementSource Source => Text is not null
        ? ElementSource.FromText(Text, Alphabet)
        : ElementSource.FromMorse(Morse ?? throw new InvalidOperationException("Call ToMorse(text), ToAudio(morse) or ToLight(morse) first."));

    /// <summary>Element durations in seconds.</summary>
    public MorseTiming Timing => new(CharSpeed, WordSpeed);

    /// <summary>Element durations in samples at the WAV sample rate.</summary>
    public SampleTiming SampleTiming => new(Timing, WavSynthesizer.SampleRate);
}
