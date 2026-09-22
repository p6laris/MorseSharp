namespace MorseSharp;

/// <summary>
/// Decodes Morse from audio arriving a piece at a time, such as from a microphone or a receiver.
/// </summary>
/// <remarks>
/// <para>
/// Push samples in with <see cref="Write"/> and take characters out with <see cref="TryRead"/>. Buffer sizes need not
/// line up with anything; whatever does not fill a whole analysis block is carried over to the next call. Call
/// <see cref="Flush"/> when the transmission ends, because the last character of a message has no gap after it to
/// announce that it finished.
/// </para>
/// <para>
/// Decoding a file can look at the whole recording before judging any of it. A live stream cannot, so both the tone
/// threshold and the sender's timing are re-measured continuously from a sliding window of what was heard recently.
/// The opening moments are the awkward part: with no silence heard yet there is nothing to compare a first tone
/// against. Rather than lose the first character, the early blocks are held back and replayed once enough has arrived
/// to judge them.
/// </para>
/// <para>
/// This type is not thread-safe. Drive one instance from one thread, or guard it yourself.
/// </para>
/// </remarks>
public sealed class StreamingMorseDecoder
{
    /// <summary>Blocks held back before the first threshold is taken, then replayed through it.</summary>
    private const int WarmupBlocks = 64;

    /// <summary>How much recent history the threshold is measured over.</summary>
    private const int PowerHistory = 512;

    /// <summary>How often the threshold is re-measured, in blocks.</summary>
    private const int RefreshInterval = 32;

    /// <summary>How many recent runs the timing is measured over.</summary>
    private const int RunHistory = 64;

    /// <summary>
    /// How many runs must be heard before the measured timing displaces the speed the caller stated. Measuring from
    /// one or two runs is worse than not measuring at all: the first dash of a message would be the only evidence
    /// available and would be taken for the dit, turning it into a dot.
    /// </summary>
    private const int MinRunsForTiming = 12;

    /// <summary>Word-gap threshold in dits, used until enough long gaps have been heard to measure the real one.</summary>
    private const float DefaultWordGapDits = 5f;

    /// <summary>Capacity of the output queue. Characters arrive slowly, so this is generous.</summary>
    private const int OutputCapacity = 512;

    /// <summary>Opens a little above the split point, and closes well below it, which gives the hysteresis.</summary>
    private const float OpenFactor = 1.10f;
    private const float CloseFactor = 0.70f;

    private readonly MorseAlphabet _alphabet;
    private readonly GoertzelDetector _detector;
    private readonly int _window;
    private readonly float _seedDit;

    private readonly short[] _carry;
    private readonly float[] _powers = new float[PowerHistory];
    private readonly int[] _runs = new int[RunHistory];
    private readonly char[] _output = new char[OutputCapacity];

    private int _carryCount;
    private int _powerCount;
    private int _powerIndex;
    private int _runCount;
    private int _runIndex;
    private int _outputHead;
    private int _outputTail;
    private int _outputCount;

    private bool _primed;
    private int _blocksSinceRefresh;
    private float _openLevel;
    private float _closeLevel;

    private bool _keyDown;
    private int _runLength;

    private MorseTimingModel _timing;
    private MorseSymbolAccumulator _symbols;

    /// <summary>Creates a decoder.</summary>
    /// <param name="alphabet">Alphabet to resolve sequences against.</param>
    /// <param name="sampleRate">Sample rate of the incoming audio, in hertz.</param>
    /// <param name="frequency">The tone frequency to listen for, in hertz.</param>
    /// <param name="wordsPerMinute">
    /// Roughly how fast the sender is keying. It seeds the analysis block size and the first timing guess; the real
    /// speed is measured from the signal, so this only has to be in the right area.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="alphabet"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown for a non-positive sample rate or speed, or an out-of-range frequency.</exception>
    internal StreamingMorseDecoder(MorseAlphabet alphabet, int sampleRate, double frequency, int wordsPerMinute)
    {
        ArgumentNullException.ThrowIfNull(alphabet);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sampleRate);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(wordsPerMinute);
        if (!(frequency > 0 && frequency < sampleRate / 2.0))
            throw new ArgumentOutOfRangeException(nameof(frequency), frequency, $"Frequency must be greater than 0 and less than {sampleRate / 2.0} Hz.");

        _alphabet = alphabet;
        _window = MorseAudioDecoder.WindowSizeFor(sampleRate, wordsPerMinute);
        _detector = new GoertzelDetector(frequency, sampleRate, _window);
        _carry = new short[_window];
        _seedDit = MorseAudioDecoder.DitBlocks(sampleRate, wordsPerMinute, _window);

        _timing = new MorseTimingModel(_seedDit, _seedDit * DefaultWordGapDits);
        _symbols = new MorseSymbolAccumulator();
    }

    /// <summary>Feeds in the next piece of audio. Any characters it completes become available to <see cref="TryRead"/>.</summary>
    /// <param name="samples">16-bit PCM mono samples, at the sample rate given to the constructor.</param>
    public void Write(ReadOnlySpan<short> samples)
    {
        while (!samples.IsEmpty)
        {
            if (_carryCount > 0 || samples.Length < _window)
            {
                int take = Math.Min(_window - _carryCount, samples.Length);
                samples[..take].CopyTo(_carry.AsSpan(_carryCount));
                _carryCount += take;
                samples = samples[take..];

                if (_carryCount < _window)
                    return;

                Consume(_carry);
                _carryCount = 0;
                continue;
            }

            Consume(samples[.._window]);
            samples = samples[_window..];
        }
    }

    /// <summary>Takes the next decoded character, if one is ready.</summary>
    /// <param name="character">The character, or <c>'\0'</c> when none is waiting.</param>
    /// <returns><c>false</c> when nothing is waiting.</returns>
    public bool TryRead(out char character)
    {
        if (_outputCount == 0)
        {
            character = '\0';
            return false;
        }

        character = _output[_outputHead];
        _outputHead = (_outputHead + 1) % OutputCapacity;
        _outputCount--;
        return true;
    }

    /// <summary>
    /// Ends the transmission, emitting the character in progress.
    /// </summary>
    /// <remarks>
    /// A character is only known to be finished once a gap follows it, so without this the final character of a
    /// message would sit unemitted for ever.
    /// </remarks>
    public void Flush()
    {
        if (!_primed)
            Prime();

        if (_runLength > 0)
        {
            if (_keyDown)
                _symbols.Add(_timing.ClassifyMark(_runLength));

            _runLength = 0;
        }

        EnqueueResolved();
        _carryCount = 0;
    }

    /// <summary>Discards all state, as if the decoder had just been created.</summary>
    public void Reset()
    {
        _carryCount = 0;
        _powerCount = 0;
        _powerIndex = 0;
        _runCount = 0;
        _runIndex = 0;
        _outputHead = _outputTail = _outputCount = 0;
        _primed = false;
        _blocksSinceRefresh = 0;
        _keyDown = false;
        _runLength = 0;
        _timing = new MorseTimingModel(_seedDit, _seedDit * DefaultWordGapDits);
        _symbols.Clear();
    }

    private void Consume(ReadOnlySpan<short> block)
    {
        float power = _detector.Power(block);
        _powers[_powerIndex] = power;
        _powerIndex = (_powerIndex + 1) % PowerHistory;
        if (_powerCount < PowerHistory)
            _powerCount++;

        if (!_primed)
        {
            // Hold the opening blocks back rather than judge them against a threshold that does not exist yet.
            if (_powerCount < WarmupBlocks)
                return;

            Prime();
            return;
        }

        if (++_blocksSinceRefresh >= RefreshInterval)
            Refresh();

        Gate(power);
    }

    /// <summary>Takes the first threshold, then replays everything that was held back through it.</summary>
    private void Prime()
    {
        if (_primed)
            return;

        _primed = true;
        Refresh();

        int held = Math.Min(_powerCount, PowerHistory);
        for (int i = 0; i < held; i++)
            Gate(_powers[i]);
    }

    private void Refresh()
    {
        _blocksSinceRefresh = 0;

        float threshold = ToneThreshold.Compute(_powers.AsSpan(0, _powerCount));
        if (threshold == float.MaxValue)
        {
            // Nothing but flat audio so far: keep the gate shut rather than invent keying.
            _openLevel = float.MaxValue;
            _closeLevel = float.MaxValue;
            return;
        }

        _openLevel = threshold * OpenFactor;
        _closeLevel = threshold * CloseFactor;
    }

    private void Gate(float power)
    {
        bool nowDown = _keyDown ? power >= _closeLevel : power > _openLevel;

        if (nowDown == _keyDown)
        {
            _runLength++;
            return;
        }

        // Silence before the first tone separates nothing, and its length would distort the timing.
        if (_runLength > 0 && (_keyDown || _runCount > 0))
            CompleteRun(_keyDown ? _runLength : -_runLength);

        _keyDown = nowDown;
        _runLength = 1;
    }

    private void CompleteRun(int run)
    {
        // Judge the run against what is currently believed, before letting the run itself change that belief.
        if (run > 0)
        {
            _symbols.Add(_timing.ClassifyMark(run));
        }
        else
        {
            MorseGap gap = _timing.ClassifyGap(-run);
            if (gap != MorseGap.Element)
            {
                EnqueueResolved();

                if (gap == MorseGap.Word)
                    Enqueue(' ');
            }
        }

        _runs[_runIndex] = run;
        _runIndex = (_runIndex + 1) % RunHistory;
        if (_runCount < RunHistory)
            _runCount++;

        Remeasure();
    }

    /// <summary>
    /// Re-derives the timing from the runs heard lately, so a sender who drifts is followed rather than fought.
    /// </summary>
    private void Remeasure()
    {
        if (_runCount < MinRunsForTiming)
            return;

        ReadOnlySpan<int> history = _runs.AsSpan(0, _runCount);
        float dit = RunStatistics.EstimateDit(history, _seedDit);
        float wordGap = RunStatistics.EstimateWordGap(history, dit);

        // No long gaps heard yet, so fall back to the standard spacing rather than treating everything as one word.
        if (wordGap == float.MaxValue)
            wordGap = dit * DefaultWordGapDits;

        _timing = new MorseTimingModel(dit, wordGap);
    }

    /// <summary>Queues whatever the accumulator resolved to, bracketing it when it is a prosign.</summary>
    private void EnqueueResolved()
    {
        if (!_symbols.TryResolve(_alphabet, out char character, out string? prosign))
            return;

        if (prosign is null)
        {
            Enqueue(character);
            return;
        }

        Enqueue(MorseTextScanner.ProsignStart);
        foreach (char letter in prosign)
            Enqueue(letter);
        Enqueue(MorseTextScanner.ProsignEnd);
    }

    private void Enqueue(char character)
    {
        if (_outputCount == OutputCapacity)
        {
            // The caller is not draining. Drop the oldest so the newest still gets through.
            _outputHead = (_outputHead + 1) % OutputCapacity;
            _outputCount--;
        }

        _output[_outputTail] = character;
        _outputTail = (_outputTail + 1) % OutputCapacity;
        _outputCount++;
    }
}
