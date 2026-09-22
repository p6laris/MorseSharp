namespace MorseSharp.Audio.Decoding;

/// <summary>What a stretch of key-up time separates.</summary>
internal enum MorseGap : byte
{
    /// <summary>A gap between the symbols of one character. Nothing is emitted.</summary>
    Element,

    /// <summary>A gap between characters. The character just collected is emitted.</summary>
    Character,

    /// <summary>A gap between words. The character is emitted, followed by a space.</summary>
    Word,
}
