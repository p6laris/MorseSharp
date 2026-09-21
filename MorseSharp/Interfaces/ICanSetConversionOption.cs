namespace MorseSharp.Interfaces;

/// <summary>
/// Step reached after choosing a language: pick what to convert.
/// </summary>
public interface ICanSetConversionOption
{
    /// <summary>
    /// Decodes Morse code to text. Characters are separated by whitespace and words by <c>/</c>.
    /// </summary>
    /// <param name="morse">The Morse code, for example <c>.... .. / - .... . .-. .</c></param>
    /// <returns>The decoded text (letters are upper case).</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="morse"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="morse"/> is empty.</exception>
    /// <exception cref="Exceptions.SequenceNotFoundException">Thrown when a sequence has no character in the selected language.</exception>
    string Decode(string morse);

    /// <summary>
    /// Decodes received audio back to text.
    /// </summary>
    /// <param name="samples">16-bit PCM mono samples.</param>
    /// <param name="sampleRate">Sample rate of the audio, in hertz.</param>
    /// <param name="frequency">The tone frequency to listen for, in hertz.</param>
    /// <param name="wordsPerMinute">
    /// Roughly how fast the sender is keying. It only has to be close: the decoder measures the real speed as it goes
    /// and keeps adjusting, which is what lets it follow hand-sent Morse.
    /// </param>
    /// <returns>The decoded text. A sequence with no character in this alphabet decodes to <c>?</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for a non-positive sample rate, frequency or speed.</exception>
    string FromAudio(ReadOnlySpan<short> samples, int sampleRate = 11025, double frequency = 700, int wordsPerMinute = 20);

    /// <summary>
    /// Creates a decoder for audio that arrives a piece at a time, such as from a microphone or a receiver.
    /// </summary>
    /// <param name="sampleRate">Sample rate of the incoming audio, in hertz.</param>
    /// <param name="frequency">The tone frequency to listen for, in hertz.</param>
    /// <param name="wordsPerMinute">Roughly how fast the sender is keying. The real speed is measured from the signal.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for a non-positive sample rate or speed, or an out-of-range frequency.</exception>
    StreamingMorseDecoder CreateAudioDecoder(int sampleRate = 11025, double frequency = 700, int wordsPerMinute = 20);

    /// <summary>
    /// Encodes text to Morse code. The text is validated immediately; the Morse string is produced lazily by
    /// <see cref="ICanGenerateAudioAndLight.Encode"/> or consumed directly by the audio and light generators.
    /// </summary>
    /// <param name="text">The text to encode. Letter case is ignored.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="text"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="text"/> is empty.</exception>
    /// <exception cref="Exceptions.CharacterNotPresentedException">Thrown when a character has no Morse code in the selected language.</exception>
    ICanGenerateAudioAndLight ToMorse(string text);

    /// <summary>
    /// Skips encoding and generates audio for Morse code you already have.
    /// </summary>
    /// <param name="morse">Dots, dashes, whitespace between characters and <c>/</c> between words.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="morse"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="morse"/> is empty or contains a symbol other than dot, dash, slash or whitespace.</exception>
    ICanSetAudioOptions ToAudio(string morse);

    /// <summary>
    /// Skips encoding and blinks a light for Morse code you already have.
    /// </summary>
    /// <param name="morse">Dots, dashes, whitespace between characters and <c>/</c> between words.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="morse"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="morse"/> is empty or contains a symbol other than dot, dash, slash or whitespace.</exception>
    ICanSetBlinkerOptions ToLight(string morse);
}
