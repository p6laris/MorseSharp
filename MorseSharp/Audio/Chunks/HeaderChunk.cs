namespace MorseSharp.Audio.Chunks;

internal readonly ref struct ValueHeaderChunk
{
    readonly ValueDataChunk _dataChunk;
    readonly ValueFormatChunk _formatChunk;
    readonly uint _chunkSize;
    readonly int _bufferSize;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueHeaderChunk(in ValueDataChunk dataChunk, in ValueFormatChunk formatChunk)
    {
        _dataChunk = dataChunk;
        _formatChunk = formatChunk;
        _chunkSize = 36 + dataChunk.GetChunkSize();
        _bufferSize = (3 * 4) + dataChunk.Capacity + formatChunk.Capacity;

    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> ToBytes()
    {
        Span<byte> riffType = stackalloc byte[4]{87, 65, 86, 69};
        Span<byte> chunkId = stackalloc byte[4] {82, 73, 70, 70};

        using SpanOwner<byte> owner = SpanOwner<byte>.Allocate(_bufferSize);
        Span<byte> data = owner.Span;

        SpanByteWriter writer = new SpanByteWriter(ref data);

        writer.AddRange(chunkId);
        writer.AddRangeBit(_chunkSize);
        writer.AddRange(riffType);
        writer.AddRange(_formatChunk.ToBytes());
        writer.AddRange(_dataChunk.ToBytes());


        return owner.Span;
    }
}