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
