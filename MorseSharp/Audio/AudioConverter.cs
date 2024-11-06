using MorseSharp.Extentions;

namespace MorseSharp.Audio;

/*
 * The AudioConverter, chunks classes are inspired by jstoddard's CWLibrary
 * Src: https://github.com/jstoddard/CWLibrary
 */

[StructLayout(LayoutKind.Sequential)]
internal readonly ref struct AudioConverter
{
    private readonly int _characterSpeed;
    private readonly int _wordSpeed;
    private readonly double _frequency;

    private readonly double _delay;
    private readonly SpanOwner<short> _preCalculatedDot;
    private readonly SpanOwner<short> _preCalculatedDash;
    private readonly SpanOwner<short> _preCalculatedSilenceBuffer;


    public AudioConverter(int characterSpeed, int wordSpeed, double frequency)
    {
        if (characterSpeed < wordSpeed)
            throw new SmallerCharSpeedException(characterSpeed, wordSpeed);

        _characterSpeed = characterSpeed;
        _wordSpeed = wordSpeed;
        _frequency = frequency;
        _delay = (60.0 / _wordSpeed) - (32.0 / _characterSpeed);

        // Precalculate dot and dash using pooled buffers
        _preCalculatedDot = SpanOwner<short>.Allocate((int)(11025 * (1.2 / _characterSpeed)), AllocationMode.Clear);
        _preCalculatedDash = SpanOwner<short>.Allocate((int)(11025 * (3.6 / _characterSpeed)), AllocationMode.Clear);
        PopulateWave(_preCalculatedDot.Span, 1.2 / _characterSpeed);
        PopulateWave(_preCalculatedDash.Span, 3.6 / _characterSpeed);

        // Precalculate a single large silence buffer to be sliced as needed
        int maxSilenceSamples = (int)(11025 * (7 * _delay / 19)); // largest silence needed (inter-word space)
        _preCalculatedSilenceBuffer = SpanOwner<short>.Allocate(maxSilenceSamples, AllocationMode.Clear);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlySpan<short> GetDot() => _preCalculatedDot.Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlySpan<short> GetDash() => _preCalculatedDash.Span;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlySpan<short> GetEleCharSpace() => GetSilence(1.2 / _characterSpeed);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlySpan<short> GetInterCharSpace() => GetSilence(3 * _delay / 19);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlySpan<short> GetInterWordSpace() => GetSilence(7 * _delay / 19);

    private void PopulateWave(Span<short> data, double seconds)
    {
        int samples = data.Length;
        double angleIncrement = 2 * Math.PI * _frequency / 11025;

        for (int i = 0; i < samples; i++)
        {
            data[i] = (short)(32760 * Math.Sin(i * angleIncrement));
        }
    }

    private ReadOnlySpan<short> GetSilence(double seconds)
    {
        int requiredSamples = (int)(11025 * seconds);
        return _preCalculatedSilenceBuffer.Span.Slice(0, requiredSamples);
    }

    private ReadOnlySpan<short> GetCharacter(ReadOnlySpan<char> morseSymbol)
    {
        using ValueListPool<short> data = new ValueListPool<short>();

        int i = 0;
        foreach (char c in morseSymbol)
        {
            if (i > 0)
                data.AddRange(GetEleCharSpace());
            if (c == '-')
                data.AddRange(GetDash());
            else if (c == '.')
                data.AddRange(GetDot());

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
                data.AddRange(GetInterWordSpace());

            data.AddRange(GetCharacter(text[range]));
            i++;
        }
        // Pad the end with a little bit of silence. Otherwise, the last character may sound funny in some media players.
        data.AddRange(GetInterCharSpace());

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
        _preCalculatedDot.Dispose();
        _preCalculatedDash.Dispose();
        _preCalculatedSilenceBuffer.Dispose();
    }
}
