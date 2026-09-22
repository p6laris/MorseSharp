namespace MorseSharp.Audio;

/// <summary>
/// How the generated audio should sound and be stored.
/// </summary>
/// <remarks>
/// <para>
/// The defaults reproduce what the library produced before these were configurable, apart from the fade, which is on
/// by default because the sound without it is worse for no benefit.
/// </para>
/// <para>
/// <see cref="EdgeMilliseconds"/> is the one that changes what you hear. Switching a tone on and off instantly makes
/// the waveform jump from full amplitude to nothing between one sample and the next, and that jump is audible as a
/// click at the start and end of every dot and dash. Fading over a few milliseconds removes it.
/// </para>
/// </remarks>
/// <param name="SampleRate">Samples per second. Must be positive, and more than twice the tone frequency.</param>
/// <param name="Channels">1 for mono or 2 for stereo. A stereo file carries the same tone in both channels.</param>
/// <param name="BitDepth">How each sample is stored.</param>
/// <param name="EdgeMilliseconds">
/// How long each dot and dash takes to fade in and out. Zero switches the tone instantly, which is what causes key
/// clicks. Anything longer than half an element is trimmed to fit.
/// </param>
public sealed record AudioFormat(
    int SampleRate = 11025,
    int Channels = 1,
    AudioBitDepth BitDepth = AudioBitDepth.Pcm16,
    double EdgeMilliseconds = 5)
{
    /// <summary>The settings used when none are given.</summary>
    public static AudioFormat Default { get; } = new();

    /// <summary>Bytes taken by one sample of one channel.</summary>
    internal int BytesPerSample => BitDepth switch
    {
        AudioBitDepth.Pcm8 => 1,
        AudioBitDepth.Float32 => 4,
        _ => 2,
    };

    /// <summary>Bytes taken by one sample across every channel.</summary>
    internal int BytesPerFrame => BytesPerSample * Channels;

    /// <summary>
    /// The byte that means silence. Zero for the signed and float formats, but 8-bit WAV samples are unsigned, so
    /// their midpoint is 128 and a buffer of zeros would be full-scale negative rather than quiet.
    /// </summary>
    internal byte SilenceByte => BitDepth == AudioBitDepth.Pcm8 ? (byte)128 : (byte)0;

    /// <summary>Throws when the settings cannot produce a playable file.</summary>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for an unusable sample rate, channel count, depth or fade.</exception>
    internal void Validate()
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(SampleRate);

        if (Channels is not (1 or 2))
            throw new ArgumentOutOfRangeException(nameof(Channels), Channels, "Channels must be 1 (mono) or 2 (stereo).");

        if (!Enum.IsDefined(BitDepth))
            throw new ArgumentOutOfRangeException(nameof(BitDepth), BitDepth, "Unsupported bit depth.");

        if (EdgeMilliseconds < 0 || double.IsNaN(EdgeMilliseconds))
            throw new ArgumentOutOfRangeException(nameof(EdgeMilliseconds), EdgeMilliseconds, "The fade cannot be negative.");
    }
}
