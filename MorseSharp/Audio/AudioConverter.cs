namespace MorseSharp.Audio;

/*
 * The AudioConverter, chunks classes are inspired by jstoddard's CWLibrary
 * Src: https://github.com/jstoddard/CWLibrary
 */

[StructLayout(LayoutKind.Sequential)]
internal readonly ref struct AudioConverter : IDisposable
{
    private readonly WaveformBuffer _buffer;
    public AudioConverter(int characterSpeed, int wordSpeed, double frequency)
    {
        if (characterSpeed < wordSpeed)
            throw new SmallerCharSpeedException(characterSpeed, wordSpeed);

        _buffer = new WaveformBuffer(characterSpeed, wordSpeed, frequency);
    }

    private ReadOnlySpan<short> GetCharacter(ReadOnlySpan<char> morseSymbol)
    {
        using ValueListPool<short> data = new ValueListPool<short>();

        int i = 0;
        foreach (char c in morseSymbol)
        {
            if (i > 0)
                data.AddRange(_buffer.GetEleCharSpace());
                
            data.AddRange(c == '-' ? _buffer.GetDash() : _buffer.GetDot());
            i++;
        }
        return data.AsSpan();
    }

    private ReadOnlySpan<short> GenerateWav(ReadOnlySpan<char> text)
    {
        using ValueListPool<short> data = new ValueListPool<short>();

        int i = 0;
        foreach (Range range in text.Split(' '))
        {
            if (i > 0)
                data.AddRange(_buffer.GetInterWordSpace());

            data.AddRange(GetCharacter(text[range]));
            i++;
        }
        data.AddRange(_buffer.GetInterCharSpace());
        return data.AsSpan();
    }

    [SkipLocalsInit]
    internal void ConvertToAudio(ReadOnlySpan<char> morse, out Span<byte> destination)
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

    public void Dispose()
    {
        _buffer.Dispose();
    }
}

