namespace MorseSharp.Core;

/// <summary>
/// Something that can replay its element stream into any sink: text plus an alphabet, or a ready Morse string.
/// Lets a consumer walk the same input twice (count, then write) without materialising anything in between.
/// </summary>
internal interface IWalkSource
{
    /// <summary>Pushes every element of the sequence into <paramref name="sink"/>, in order.</summary>
    void Walk<TSink>(ref TSink sink) where TSink : struct, IElementSink, allows ref struct;
}
