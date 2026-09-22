using MorseSharp;

namespace ConsoleExample.Demos;

/// <summary>Shared helper for demos that need raw PCM rather than a whole WAV file.</summary>
internal static class AudioSamples
{
    /// <summary>
    /// Renders 16-bit mono PCM directly, which is what the decoders read. <c>GetBytes()</c> produces
    /// a whole WAV file instead, so the samples are pulled from the buffer past the 44-byte header.
    /// </summary>
    public static short[] Render16BitMono(string text, int charSpeed, int wordSpeed, double frequency)
    {
        byte[] wav = Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse(text)
            .ToAudio()
            .SetAudioOptions(charSpeed, wordSpeed, frequency)
            .GetBytes();

        short[] pcm = new short[(wav.Length - 44) / 2];
        Buffer.BlockCopy(wav, 44, pcm, 0, pcm.Length * 2);
        return pcm;
    }
}
