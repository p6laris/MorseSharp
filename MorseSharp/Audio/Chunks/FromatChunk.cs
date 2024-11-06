using System.IO.Compression;

namespace MorseSharp.Audio.Chunks;

[StructLayout(LayoutKind.Sequential)]
internal readonly ref struct ValueFormatChunk
{
    private readonly short _compressionCode;
    private readonly short _numChannels;
    private readonly uint _sampleRate;
    private readonly uint _bytesPerSecond;
    private readonly short _blockAlign;
    private readonly short _significantBits;
    private readonly uint _chunkSize;


    public ValueFormatChunk()
    {
        _compressionCode = 1;      // PCM
        _numChannels = 1;          // Mono
        _sampleRate = 11025;       // 11.025 kHz
        _bytesPerSecond = 22050;   // SampleRate * BlockAlign
        _blockAlign = 2;           // Mono, 16 bits = 2 bytes
        _significantBits = 16;     // 16-bit audio
        _chunkSize = 16;           // Fixed for PCM
    }

    public const int Capacity = 24;  // Chunk ID (4) + ChunkSize (4) + Format data (16)

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> ToBytes()
    {
        ReadOnlySpan<byte> chunkId = [102, 109, 116, 32]; // 'fmt ' chunk ID

        using SpanOwner<byte> owner = SpanOwner<byte>.Allocate(Capacity);
        Span<byte> data = owner.Span;

        SpanByteWriter writer = new SpanByteWriter(ref data);

        writer.AddRange(chunkId);
        writer.AddRangeBit(_chunkSize);
        writer.AddRangeBit(_compressionCode);
        writer.AddRangeBit(_numChannels);
        writer.AddRangeBit(_sampleRate);
        writer.AddRangeBit(_bytesPerSecond);
        writer.AddRangeBit(_blockAlign);
        writer.AddRangeBit(_significantBits);

        return owner.Span;
    }

}