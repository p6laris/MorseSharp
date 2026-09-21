namespace MorseSharp.Core;

/// <summary>
/// The element source of the current chain: either text that is encoded through an alphabet on the fly,
/// or a Morse string that is walked directly.
/// </summary>
internal readonly struct ElementSource : IWalkSource
{
    private readonly string _content;
    private readonly MorseAlphabet? _alphabet;

    private ElementSource(string content, MorseAlphabet? alphabet)
    {
        _content = content;
        _alphabet = alphabet;
    }

    /// <summary>Walks text, encoding each character through <paramref name="alphabet"/>.</summary>
    public static ElementSource FromText(string text, MorseAlphabet alphabet) => new(text, alphabet);

    /// <summary>Walks a string of dots, dashes, whitespace and slashes.</summary>
    public static ElementSource FromMorse(string morse) => new(morse, null);

    /// <inheritdoc />
    public void Walk<TSink>(ref TSink sink) where TSink : struct, IElementSink, allows ref struct
    {
        if (_alphabet is not null)
            MorseWalker.WalkText(_content, _alphabet, ref sink);
        else
            MorseWalker.WalkMorse(_content, ref sink);
    }
}
