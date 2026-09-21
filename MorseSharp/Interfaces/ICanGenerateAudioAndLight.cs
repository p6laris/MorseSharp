namespace MorseSharp.Interfaces;

/// <summary>
/// Step reached after <see cref="ICanSetConversionOption.ToMorse"/>: read the Morse string or continue to audio/light.
/// </summary>
public interface ICanGenerateAudioAndLight
{
    /// <summary>
    /// Returns the Morse code for the text passed to <see cref="ICanSetConversionOption.ToMorse"/>.
    /// Characters are separated by a single space and words by <c>/</c>.
    /// </summary>
    string Encode();

    /// <summary>Continues to audio generation for the encoded text.</summary>
    ICanSetAudioOptions ToAudio();

    /// <summary>Continues to light blinking for the encoded text.</summary>
    ICanSetBlinkerOptions ToLight();
}
