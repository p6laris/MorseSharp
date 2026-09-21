namespace MorseSharp.Alphabet;

/// <summary>
/// Reads text one keyable token at a time: either a single character, or a bracketed prosign such as <c>&lt;AR&gt;</c>.
/// </summary>
/// <remarks>
/// Encoding used to step character by character, which cannot express a prosign: its letters are keyed as one
/// unbroken signal, so <c>&lt;AR&gt;</c> is a single token rather than four characters. Every path that encodes text
/// drives this, so the three of them cannot disagree about where one token ends and the next begins.
/// </remarks>
internal static class MorseTextScanner
{
    /// <summary>Opens a prosign.</summary>
    public const char ProsignStart = '<';

    /// <summary>Closes a prosign.</summary>
    public const char ProsignEnd = '>';

    /// <summary>
    /// Reads the token starting at <paramref name="position"/> and advances past it.
    /// </summary>
    /// <param name="text">The text being encoded.</param>
    /// <param name="position">Where to read from; moved past the token on return.</param>
    /// <param name="alphabet">Alphabet to resolve against.</param>
    /// <returns>The tree code of the token.</returns>
    /// <exception cref="CharacterNotPresentedException">Thrown when a character has no pattern in the alphabet.</exception>
    /// <exception cref="ProsignNotPresentedException">Thrown when a bracketed prosign is not defined.</exception>
    public static int Next(ReadOnlySpan<char> text, ref int position, MorseAlphabet alphabet)
    {
        char first = text[position];

        if (first == ProsignStart)
        {
            int close = text[position..].IndexOf(ProsignEnd);
            if (close > 1)
            {
                ReadOnlySpan<char> name = text.Slice(position + 1, close - 1);
                if (alphabet.TryGetProsignCode(name, out int prosignCode))
                {
                    position += close + 1;
                    return prosignCode;
                }

                throw new ProsignNotPresentedException(name.ToString(), alphabet.Name);
            }
        }

        // Not a prosign, so an ordinary character. A stray '<' falls through to here and is reported as itself.
        if (!alphabet.TryGetCode(first, out int code))
            throw new CharacterNotPresentedException(first, alphabet.Name);

        position++;
        return code;
    }
}
