namespace MorseSharp;

/// <summary>
/// Walks a Morse sequence one element at a time.
/// </summary>
/// <remarks>
/// This is <see cref="MorseWalker"/> turned inside out: where the walker pushes into a sink, this returns to the
/// caller. It holds nothing but a string reference and a few integers, so unlike a span-based walk it can live
/// across an <c>await</c>, and as a struct reached through <c>foreach</c> it never allocates.
/// </remarks>
public struct MorseElementEnumerator
{
    private enum Phase : byte
    {
        NextChar,
        Symbol,
        ElementGap,
        CharGap,
        Done,
    }

    private readonly string _content;
    private readonly MorseAlphabet? _alphabet;
    private readonly MorseTiming _timing;

    private int _position;
    private int _code;
    private int _bit;
    private int _runEnd;
    private Phase _phase;
    private bool _needCharGap;

    internal MorseElementEnumerator(string content, MorseAlphabet? alphabet, MorseTiming timing)
    {
        _content = content;
        _alphabet = alphabet;
        _timing = timing;
        Current = default;
    }

    /// <summary>The element produced by the last <see cref="MoveNext"/>.</summary>
    public MorseElement Current { get; private set; }

    /// <summary>Produces the next element, returning <c>false</c> once the sequence is finished.</summary>
    public bool MoveNext()
    {
        while (true)
        {
            switch (_phase)
            {
                case Phase.Symbol:
                    return Emit(NextSymbol());

                case Phase.ElementGap:
                    _phase = Phase.Symbol;
                    return Emit(MorseElementKind.ElementGap);

                case Phase.CharGap:
                    _phase = Phase.Symbol;
                    return Emit(MorseElementKind.CharGap);

                case Phase.Done:
                    return false;

                default:
                    if (_position >= _content.Length)
                    {
                        // A trailing character gap, so audio never stops abruptly and a light always ends switched off.
                        _phase = Phase.Done;
                        return Emit(MorseElementKind.CharGap);
                    }

                    if (!StartNextCharacter())
                        continue;

                    return Emit(MorseElementKind.WordGap);
            }
        }
    }

    /// <summary>
    /// Positions the enumerator on the next keyed character. Returns <c>true</c> when a word gap should be emitted
    /// instead, and leaves the phase unchanged so the scan resumes from the following character.
    /// </summary>
    private bool StartNextCharacter()
    {
        if (_alphabet is null)
        {
            char symbol = _content[_position];

            if (char.IsWhiteSpace(symbol))
            {
                // A separator on its own; the character gap is emitted when the next run of symbols starts.
                _position++;
                _phase = Phase.NextChar;
                return false;
            }

            if (symbol == '/')
            {
                _position++;
                _needCharGap = false;
                return true;
            }

            _runEnd = _position;
            while (_runEnd < _content.Length && _content[_runEnd] is '.' or '-')
                _runEnd++;
        }
        else
        {
            int code = MorseTextScanner.Next(_content, ref _position, _alphabet);
            if (code == MorseAlphabet.WordSpaceCode)
            {
                _needCharGap = false;
                return true;
            }

            _code = code;
            _bit = MorseAlphabet.SymbolCount(code) - 1;
        }

        _phase = _needCharGap ? Phase.CharGap : Phase.Symbol;
        return false;
    }

    /// <summary>Takes one symbol off the current character and decides what follows it.</summary>
    private MorseElementKind NextSymbol()
    {
        bool dash;
        if (_alphabet is null)
        {
            dash = _content[_position] == '-';
            _position++;

            if (_position < _runEnd)
            {
                _phase = Phase.ElementGap;
            }
            else
            {
                _needCharGap = true;
                _phase = Phase.NextChar;
            }
        }
        else
        {
            dash = ((_code >> _bit) & 1) != 0;

            if (_bit == 0)
            {
                _needCharGap = true;
                _phase = Phase.NextChar;
            }
            else
            {
                _bit--;
                _phase = Phase.ElementGap;
            }
        }

        return dash ? MorseElementKind.Dash : MorseElementKind.Dot;
    }

    private bool Emit(MorseElementKind kind)
    {
        // Ticks rather than TimeSpan.FromSeconds, which rounds to the nearest millisecond and would drift at high speeds.
        Current = new MorseElement(kind, TimeSpan.FromTicks((long)(_timing.For(kind) * TimeSpan.TicksPerSecond)));
        return true;
    }
}
