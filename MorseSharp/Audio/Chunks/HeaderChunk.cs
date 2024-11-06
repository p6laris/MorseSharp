namespace MorseSharp.Audio.Chunks;

[StructLayout(LayoutKind.Sequential)]
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
        _chunkSize = 36 + dataChunk.GetChunkSize(); // 36 is the size of the rest of the RIFF header
        _bufferSize = 12 + dataChunk.Capacity + ValueFormatChunk.Capacity; // RIFF header (12) + data + format

    }

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> ToBytes()
    {
        ReadOnlySpan<byte> riffType = [87, 65, 86, 69];  // 'WAVE'
        ReadOnlySpan<byte> chunkId = [82, 73, 70, 70];  // 'RIFF'

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