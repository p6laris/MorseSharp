namespace MorseSharp.Core;

/// <summary>
/// Turns text (through an alphabet) or a Morse string into a stream of elements.
/// </summary>
/// <remarks>
/// Gaps are emitted only between keyed characters: a character gap precedes every character except the first one
/// of a word, a word gap replaces it after a space, and a trailing character gap pads the end of the sequence so that
/// audio does not stop abruptly and a light always ends switched off.
/// </remarks>
internal static class MorseWalker
{
    /// <summary>Walks <paramref name="text"/>, throwing when a character is not in <paramref name="alphabet"/>.</summary>
    public static void WalkText<TSink>(ReadOnlySpan<char> text, MorseAlphabet alphabet, ref TSink sink)
        where TSink : struct, IElementSink, allows ref struct
    {
        bool needCharGap = false;
        foreach (char ch in text)
        {
            if (!alphabet.TryGetCode(ch, out int code))
                throw new CharacterNotPresentedException(ch, alphabet.Language);

            if (code == MorseAlphabet.WordSpaceCode)
            {
                sink.WordGap();
                needCharGap = false;
                continue;
            }

            if (needCharGap)
                sink.CharGap();

            EmitCode(code, ref sink);
            needCharGap = true;
        }

        sink.CharGap();
    }

    /// <summary>
    /// Walks a Morse string made of dots, dashes, whitespace (character separator) and <c>/</c> (word separator).
    /// Any other symbol throws.
    /// </summary>
    public static void WalkMorse<TSink>(ReadOnlySpan<char> morse, ref TSink sink)
        where TSink : struct, IElementSink, allows ref struct
    {
        bool needCharGap = false;
        int i = 0;
        while (i < morse.Length)
        {
            char ch = morse[i];
            if (ch == ' ' || char.IsWhiteSpace(ch))
            {
                i++;
                continue;
            }

            if (ch == '/')
            {
                sink.WordGap();
                needCharGap = false;
                i++;
                continue;
            }

            if (ch is not ('.' or '-'))
                ThrowInvalidSymbol(morse, i);

            if (needCharGap)
                sink.CharGap();

            bool first = true;
            do
            {
                if (!first)
                    sink.ElementGap();
                first = false;

                if (ch == '-')
                    sink.Dash();
                else
                    sink.Dot();

                i++;
                if (i == morse.Length)
                    break;
                ch = morse[i];
            }
            while (ch is '.' or '-');

            needCharGap = true;
        }

        sink.CharGap();
    }

    /// <summary>Returns the index of the first symbol <see cref="WalkMorse"/> would reject, or -1.</summary>
    public static int IndexOfInvalidSymbol(ReadOnlySpan<char> morse)
    {
        for (int i = 0; i < morse.Length; i++)
        {
            char ch = morse[i];
            if (ch is '.' or '-' or '/' or ' ')
                continue;
            if (!char.IsWhiteSpace(ch))
                return i;
        }
        return -1;
    }

    /// <summary>Builds the message for a symbol that is neither a dot, a dash, a slash nor whitespace.</summary>
    public static string InvalidSymbolMessage(char symbol, int index)
        => $"Invalid Morse symbol '{symbol}' at index {index}. Only '.', '-', '/' and whitespace are allowed.";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EmitCode<TSink>(int code, ref TSink sink)
        where TSink : struct, IElementSink, allows ref struct
    {
        int i = MorseAlphabet.SymbolCount(code) - 1;
        while (true)
        {
            if (((code >> i) & 1) != 0)
                sink.Dash();
            else
                sink.Dot();

            if (i == 0)
                return;
            i--;
            sink.ElementGap();
        }
    }

    private static void ThrowInvalidSymbol(ReadOnlySpan<char> morse, int index)
        => throw new ArgumentException(InvalidSymbolMessage(morse[index], index), nameof(morse));
}
