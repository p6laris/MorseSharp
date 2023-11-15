using MorseSharp.Audio.Chunks;

namespace MorseSharp.Audio;

[StructLayout(LayoutKind.Sequential)]
public ref struct AudioConverter
{

    private readonly int _characterSpeed;
    private readonly int _wordSpeed;
    private readonly double _frequency;

    public AudioConverter(int characterSpeed, int wordSpeed, double frequency)
    {
        if (characterSpeed < wordSpeed)
            throw new SmallerWordSpeedException(characterSpeed, wordSpeed);


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
        List<short> data = new List<short>();

        for (int i = 0; i < morseSymbol.Length; i++)
        {
            if (i > 0)
                data.AddRange(GetEleCharSpace());
            if (morseSymbol[i] == '_')
                data.AddRange(GetDash());
            else if (morseSymbol[i] == '.')
                data.AddRange(GetDot());
        }

        return CollectionsMarshal.AsSpan(data);
    }

    private Span<short> GenerateWav(string text)
    {
        List<short> data = new List<short>();

        ReadOnlySpan<char> sMorse = text.AsSpan();
        Span<Range> splitedRange = new Span<Range>(new Range[sMorse.Count(' ') + 1]);

        sMorse.Split(splitedRange, ' ', StringSplitOptions.None);

        for (int i = 0; i < splitedRange.Length; i++)
        {
            var morseSymbol = sMorse.Slice(splitedRange[i].Start.Value, splitedRange[i].End.Value - splitedRange[i].Start.Value);
            if (i > 0)
                data.AddRange(GetInterWordSpace());

            data.AddRange(GetCharacter(morseSymbol.ToString()));
        }
        // Pad the end with a little bit of silence. Otherwise, the last character may sound funny in some media players.
        data.AddRange(GetInterCharSpace());

        return CollectionsMarshal.AsSpan(data);
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