namespace MorseSharp.Audio;

[StructLayout(LayoutKind.Sequential)]
internal ref struct AudioConverter
{

    private readonly int _characterSpeed;
    private readonly int _wordSpeed;
    private readonly double _frequency;

    public AudioConverter(int characterSpeed, int wordSpeed, double frequency)
    {
        if (characterSpeed < wordSpeed)
            throw new SmallerCharSpeedException(characterSpeed, wordSpeed);


        _characterSpeed = characterSpeed;
        _wordSpeed = wordSpeed;
        _frequency = frequency;

    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<short> GetDot() => GetWave(1.2 / _characterSpeed);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<short> GetDash() => GetWave(3.6 / _characterSpeed);


    private Span<short> GetEleCharSpace() => GetSilence(1.2 / _characterSpeed);

    private Span<short> GetInterCharSpace()
    {
        double delay = (60.0 / _wordSpeed) - (32.0 / _characterSpeed);
        double spaceLength = 3 * delay / 19;
        return GetSilence(spaceLength);
    }

    private Span<short> GetInterWordSpace()
    {
        double delay = (60.0 / _wordSpeed) - (32.0 / _characterSpeed);
        double spaceLength = 7 * delay / 19;
        return GetSilence(spaceLength);
    }
    [SkipLocalsInit]
    private Span<short> GetWave(double seconds)
    {
        int samples = (int)(11025 * seconds);
        using SpanOwner<short> owner = SpanOwner<short>.Allocate(samples);
        Span<short> data = owner.Span;

        for (int i = 0; i < samples; i++)
        {
            data[i] = Convert.ToInt16(32760 * Math.Sin(i * 2 * Math.PI * _frequency / 11025));
        }

        return data;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<short> GetSilence(double seconds)
    {
        int samples = (int)(11025 * seconds);
        using SpanOwner<short> owner = SpanOwner<short>.Allocate(samples, AllocationMode.Clear);
        Span<short> data = owner.Span;

        return data;

    }

    private Span<short> GetCharacter(string morseSymbol)
    {
        using ListPool<short> data = new ListPool<short>();

        for (int i = 0; i < morseSymbol.Length; i++)
        {
            if (i > 0)
                data.AddRange(GetEleCharSpace());
            if (morseSymbol[i] == '_')
                data.AddRange(GetDash());
            else if (morseSymbol[i] == '.')
                data.AddRange(GetDot());
        }

        return data.AsSpan();
    }

    private Span<short> GenerateWav(string text)
    {
        using ListPool<short> data = new ListPool<short>();

        int count = 0;
        unsafe
        {
            fixed (char* ptr = text)
            {
                // Count occurrences of spaces to split Morse code into words
                count = StringCounter.CountOccurrences(ptr, ' ');
            }

        }

        Range[]? rngArray = null;
        Span<Range> splitedRange = count + 1 < 256
            ? stackalloc Range[count + 1] : rngArray = ArrayPool<Range>.Shared.Rent(count + 1);

        text.AsSpan().Split(splitedRange, ' ', StringSplitOptions.None);

        for (int i = 0; i < splitedRange.Length; i++)
        {
            var morseSymbol = text.AsSpan().Slice(splitedRange[i].Start.Value, splitedRange[i].End.Value - splitedRange[i].Start.Value);
            if (i > 0)
                data.AddRange(GetInterWordSpace());

            data.AddRange(GetCharacter(morseSymbol.ToString()));
        }
        // Pad the end with a little bit of silence. Otherwise, the last character may sound funny in some media players.
        data.AddRange(GetInterCharSpace());

        if (rngArray is not null)
            ArrayPool<Range>.Shared.Return(rngArray);
        return data.AsSpan();
    }

    [SkipLocalsInit]
    internal void ConvertToAudio(string morse, out Span<byte> destination)
    {
        var data = GenerateWav(morse);
        ValueDataChunk dataChunk = new ValueDataChunk(data);

        ValueFormatChunk formatChunk = new ValueFormatChunk();
        ValueHeaderChunk headerChunk = new ValueHeaderChunk(in dataChunk, in formatChunk);

        Span<byte> bytes = headerChunk.ToBytes();
        using SpanOwner<byte> owner = SpanOwner<byte>.Allocate(bytes.Length);
        destination = owner.Span;

        bytes.CopyTo(destination);
    }


}