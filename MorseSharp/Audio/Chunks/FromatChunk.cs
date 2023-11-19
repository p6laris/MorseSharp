namespace MorseSharp.Audio.Chunks;

[StructLayout(LayoutKind.Sequential)]
internal readonly ref struct ValueFormatChunk
{
    private readonly short CompressionCode;
    private readonly short NumChannels;
    private readonly uint SampleRate;
    private readonly uint BytesPerSecond;
    private readonly short BlockAlign;
    private readonly short SignificantBits;
    private readonly uint ChunkSize;


    public ValueFormatChunk()
    {
        CompressionCode = 1;
        NumChannels = 1;
        SampleRate = 11025;
        BytesPerSecond = 22050;
        BlockAlign = 2;
        SignificantBits = 16;
        ChunkSize = 16;
    }

    public int Capacity => (4 * 4) + (2 * 4);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> ToBytes()
    {
        Span<byte> chunkId = stackalloc byte[4]{102, 109, 116, 32};

        using SpanOwner<byte> owner = SpanOwner<byte>.Allocate(Capacity);
        Span<byte> data = owner.Span;

        SpanByteWriter writer = new SpanByteWriter(ref data);

        writer.AddRange(chunkId);
        writer.AddRangeBit(ChunkSize);
        writer.AddRangeBit(CompressionCode);
        writer.AddRangeBit(NumChannels);
        writer.AddRangeBit(SampleRate);
        writer.AddRangeBit(BytesPerSecond);
        writer.AddRangeBit(BlockAlign);
        writer.AddRangeBit(SignificantBits);

        return owner.Span;
    }

}