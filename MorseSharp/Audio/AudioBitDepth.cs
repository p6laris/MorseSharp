using System.Diagnostics.CodeAnalysis;

namespace MorseSharp.Audio;

/// <summary>How each audio sample is stored in the WAV file.</summary>
[SuppressMessage("Naming", "CA1720:Identifier contains type name",
    Justification = "Float32 is what the audio world calls this format; renaming it would be less clear, not more.")]
public enum AudioBitDepth : byte
{
    /// <summary>
    /// 8-bit unsigned, halving the file size. Morse is a single tone, so the loss of resolution is barely audible.
    /// </summary>
    Pcm8,

    /// <summary>16-bit signed. The usual choice, and what everything can play.</summary>
    Pcm16,

    /// <summary>
    /// 32-bit IEEE float. What audio pipelines and game engines work in natively, so it saves them a conversion.
    /// </summary>
    Float32,
}
