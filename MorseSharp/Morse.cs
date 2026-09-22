namespace MorseSharp;

/// <summary>
/// Fluent entry point for encoding and decoding Morse code, rendering it as WAV audio and blinking a light.
/// </summary>
/// <remarks>
/// <para>
/// <see cref="GetConverter"/> returns a stateless singleton; the state of a chain (language, text, timing) lives in
/// thread-static storage. A chain must therefore be completed on the thread that started it, which the fluent API does
/// naturally, and starting a new chain on the same thread replaces the previous one. Using the converter concurrently
/// from many threads is safe.
/// </para>
/// <para>
/// Encoding, decoding and audio rendering allocate nothing except the returned string or array; the span and stream
/// overloads of the audio step allocate nothing at all.
/// </para>
/// </remarks>
public sealed class Morse : ICanSpecifyLanguage, ICanSetConversionOption, ICanGenerateAudioAndLight,
    ICanSetAudioOptions, ICanConvertToAudio, ICanSetBlinkerOptions, ICanConvertToLight
{
    /// <summary>Longest decode output that is built on the stack rather than from the array pool.</summary>
    private const int StackallocCharLimit = 256;

    private static readonly Morse s_instance = new();

    [ThreadStatic]
    private static ChainState? t_state;

    private static ChainState State
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => t_state ??= new ChainState();
    }

    private Morse()
    {
    }

    /// <summary>
    /// Returns the converter. Continue with <see cref="ForLanguage"/>.
    /// </summary>
    public static ICanSpecifyLanguage GetConverter() => s_instance;

    /// <inheritdoc />
    public ICanSetConversionOption ForLanguage(Language language)
    {
        State.Alphabet = Alphabets.For(language);
        return this;
    }

    /// <inheritdoc />
    public ICanSetConversionOption ForAlphabet(MorseAlphabet alphabet)
    {
        ArgumentNullException.ThrowIfNull(alphabet);
        State.Alphabet = alphabet;
        return this;
    }

    /// <inheritdoc />
    [SkipLocalsInit]
    public string Decode(string morse)
    {
        ArgumentException.ThrowIfNullOrEmpty(morse);
        MorseAlphabet alphabet = State.Alphabet;

        // Every output character consumes at least one input character, so the input length is a safe upper bound.
        // A character never outgrows its sequence, but a prosign does: two symbols can come back as <AR>.
        int capacity = morse.Length * alphabet.MaxDecodedTokenLength;

        char[]? rented = null;
        Span<char> output = capacity <= StackallocCharLimit
            ? stackalloc char[StackallocCharLimit]
            : (rented = ArrayPool<char>.Shared.Rent(capacity));

        try
        {
            int written = DecodeCore(morse, alphabet, output);
            return new string(output[..written]);
        }
        finally
        {
            if (rented is not null)
                ArrayPool<char>.Shared.Return(rented);
        }
    }

    /// <summary>
    /// Folds each whitespace-delimited sequence into its tree code and resolves it with a single table index.
    /// </summary>
    private static int DecodeCore(ReadOnlySpan<char> source, MorseAlphabet alphabet, Span<char> output)
    {
        int written = 0;
        int i = 0;
        while (i < source.Length)
        {
            char ch = source[i];
            if (ch == ' ' || char.IsWhiteSpace(ch))
            {
                i++;
                continue;
            }

            if (ch == '/')
            {
                output[written++] = ' ';
                i++;
                continue;
            }

            int start = i;
            int code = 1;
            while (true)
            {
                if (ch == '.')
                {
                    // Sequences longer than the table can hold stop growing; the lookup below then fails cleanly.
                    if (code < MorseAlphabet.CodeLimit)
                        code <<= 1;
                }
                else if (ch == '-')
                {
                    if (code < MorseAlphabet.CodeLimit)
                        code = (code << 1) | 1;
                }
                else
                {
                    break;
                }

                i++;
                if (i == source.Length)
                    break;
                ch = source[i];
            }

            if (i < source.Length && ch != '/' && !char.IsWhiteSpace(ch))
            {
                // A foreign symbol inside the sequence: report the whole whitespace-delimited token.
                while (i < source.Length && !char.IsWhiteSpace(source[i]))
                    i++;
                throw new SequenceNotFoundException(source[start..i], alphabet.Name);
            }

            char decoded = alphabet.Decode(code);
            if (decoded != '\0')
            {
                output[written++] = decoded;
                continue;
            }

            // No character owns this pattern, so a prosign may. It is written back the way it was keyed in.
            if (alphabet.TryGetProsignName(code, out string prosign))
            {
                output[written++] = MorseTextScanner.ProsignStart;
                prosign.AsSpan().CopyTo(output[written..]);
                written += prosign.Length;
                output[written++] = MorseTextScanner.ProsignEnd;
                continue;
            }

            throw new SequenceNotFoundException(source[start..i], alphabet.Name);
        }

        return written;
    }

    /// <inheritdoc />
    public string FromAudio(ReadOnlySpan<short> samples, int sampleRate = 11025, double frequency = 700, int wordsPerMinute = 20)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(sampleRate);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(wordsPerMinute);
        if (!(frequency > 0 && frequency < sampleRate / 2.0))
            throw new ArgumentOutOfRangeException(nameof(frequency), frequency, $"Frequency must be greater than 0 and less than {sampleRate / 2.0} Hz.");

        return MorseAudioDecoder.Decode(samples, State.Alphabet, sampleRate, frequency, wordsPerMinute);
    }

    /// <inheritdoc />
    public StreamingMorseDecoder CreateAudioDecoder(int sampleRate = 11025, double frequency = 700, int wordsPerMinute = 20)
        => new(State.Alphabet, sampleRate, frequency, wordsPerMinute);

    /// <inheritdoc />
    public ICanGenerateAudioAndLight ToMorse(string text)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);
        ChainState state = State;
        MorseAlphabet alphabet = state.Alphabet;

        // Counted in tokens rather than characters: a bracketed prosign is one keyed signal, however long it reads.
        long length = 0;
        int tokens = 0;
        for (int position = 0; position < text.Length; tokens++)
            length += MorseAlphabet.WrittenLength(MorseTextScanner.Next(text, ref position, alphabet));

        length += tokens - 1; // one separator between every pair of tokens

        state.Text = text;
        state.Morse = null;
        state.EncodedLength = checked((int)length);
        return this;
    }

    /// <inheritdoc />
    public string Encode()
    {
        ChainState state = State;
        if (state.Text is null)
            return state.Morse ?? throw new InvalidOperationException("Call ToMorse(text) before Encode().");

        return string.Create(state.EncodedLength, (state.Text, state.Alphabet), static (destination, tuple) =>
        {
            (string text, MorseAlphabet alphabet) = tuple;
            int read = 0;
            int written = 0;
            bool first = true;

            while (read < text.Length)
            {
                int code = MorseTextScanner.Next(text, ref read, alphabet);

                if (!first)
                    destination[written++] = ' ';
                first = false;

                MorseAlphabet.WriteCode(destination, ref written, code);
            }
        });
    }

    /// <inheritdoc />
    public ICanSetAudioOptions ToAudio() => this;

    /// <inheritdoc />
    public ICanSetBlinkerOptions ToLight() => this;

    /// <inheritdoc />
    public ICanSetAudioOptions ToAudio(string morse)
    {
        SetMorse(morse);
        return this;
    }

    /// <inheritdoc />
    public ICanSetBlinkerOptions ToLight(string morse)
    {
        SetMorse(morse);
        return this;
    }

    private static void SetMorse(string morse)
    {
        ArgumentException.ThrowIfNullOrEmpty(morse);
        int invalid = MorseWalker.IndexOfInvalidSymbol(morse);
        if (invalid >= 0)
            throw new ArgumentException(MorseWalker.InvalidSymbolMessage(morse[invalid], invalid), nameof(morse));

        ChainState state = State;
        state.Morse = morse;
        state.Text = null;
    }

    /// <inheritdoc />
    public ICanConvertToAudio SetAudioOptions(int charSpeed = 25, int wordSpeed = 25, double frequency = 700, AudioFormat? format = null)
    {
        AudioFormat chosen = format ?? AudioFormat.Default;
        chosen.Validate();
        MorseTiming.Validate(charSpeed, wordSpeed);
        WavSynthesizer.ValidateFrequency(frequency, chosen.SampleRate);

        ChainState state = State;
        state.CharSpeed = charSpeed;
        state.WordSpeed = wordSpeed;
        state.Frequency = frequency;
        state.Format = chosen;
        return this;
    }

    /// <inheritdoc />
    public ICanConvertToLight SetBlinkerOptions(int charSpeed = 25, int wordSpeed = 25)
    {
        MorseTiming.Validate(charSpeed, wordSpeed);

        ChainState state = State;
        state.CharSpeed = charSpeed;
        state.WordSpeed = wordSpeed;
        return this;
    }

    /// <inheritdoc />
    public int GetByteCount()
    {
        ChainState state = State;
        return WavSynthesizer.ByteCount(WavSynthesizer.CountSamples(state.SampleTiming, state.Source), state.Format);
    }

    /// <inheritdoc />
    public byte[] GetBytes()
    {
        ChainState state = State;
        SampleTiming timing = state.SampleTiming;
        ElementSource source = state.Source;

        int size = WavSynthesizer.ByteCount(WavSynthesizer.CountSamples(timing, source), state.Format);
        byte[] result = GC.AllocateUninitializedArray<byte>(size);
        WavSynthesizer.Render(result, timing, state.Frequency, state.Format, source);
        return result;
    }

    /// <inheritdoc />
    public int GetBytes(Span<byte> destination)
    {
        ChainState state = State;
        SampleTiming timing = state.SampleTiming;
        ElementSource source = state.Source;

        int size = WavSynthesizer.ByteCount(WavSynthesizer.CountSamples(timing, source), state.Format);
        if (destination.Length < size)
            throw new ArgumentException($"The destination holds {destination.Length} bytes but the WAV file needs {size}. Call GetByteCount() to size it.", nameof(destination));

        WavSynthesizer.Render(destination[..size], timing, state.Frequency, state.Format, source);
        return size;
    }

    /// <inheritdoc />
    public void WriteTo(Stream destination)
    {
        ArgumentNullException.ThrowIfNull(destination);

        ChainState state = State;
        SampleTiming timing = state.SampleTiming;
        ElementSource source = state.Source;

        int size = WavSynthesizer.ByteCount(WavSynthesizer.CountSamples(timing, source), state.Format);
        byte[] buffer = ArrayPool<byte>.Shared.Rent(size);
        try
        {
            WavSynthesizer.Render(buffer.AsSpan(0, size), timing, state.Frequency, state.Format, source);
            destination.Write(buffer, 0, size);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    /// <inheritdoc />
    [Obsolete("Use GetBytes() (returns byte[]) or GetBytes(Span<byte>) to fill your own buffer. This overload allocates a new array on every call.")]
    public void GetBytes(out Span<byte> destination) => destination = GetBytes();

    /// <inheritdoc />
    public MorseElementSequence GetElements() => State.Elements;

    /// <inheritdoc />
    public IAsyncEnumerable<MorseElement> PlayAsync(CancellationToken cancellationToken = default)
        => new MorseElementStream(State.Elements, TimeProvider.System, cancellationToken);

    /// <inheritdoc />
    public async Task DoBlinks(Action<bool> blinkerAction, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(blinkerAction);

        try
        {
            await foreach (MorseElement element in PlayAsync(cancellationToken))
                blinkerAction(element.KeyDown);
        }
        catch (OperationCanceledException)
        {
            blinkerAction(false);
            throw;
        }
    }
}
