namespace MorseSharp.Audio.Chunks;

// Represents a data chunk for WAVE files.
[StructLayout(LayoutKind.Sequential)]
internal readonly ref struct ValueDataChunk
{

    private readonly ReadOnlySpan<short> _chunkData;
    private readonly uint _chunkSize;

    
    public ValueDataChunk(ReadOnlySpan<short> data)
    {
        _chunkData = data;
        _chunkSize = (uint)(data.Length * 2); // Each short is 2 bytes
    }

    public int Capacity 
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => 8 + (_chunkData.Length * 2); // Header (4 bytes) + chunkSize (4 bytes) + data
    }
    
    public uint GetChunkSize() => _chunkSize;

    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> ToBytes()
    {
        ReadOnlySpan<byte> chunkId = [100, 97, 116, 97]; // 'data' chunk ID

        using SpanOwner<byte> owner = SpanOwner<byte>.Allocate(Capacity);
        Span<byte> data = owner.Span;

        SpanByteWriter writer = new SpanByteWriter(ref data);
        writer.AddRange(chunkId);
        writer.AddRangeBit(_chunkSize);

        foreach (var chunk in _chunkData)
        {
            writer.AddRangeBit(chunk);
        }

        return owner.Span;
    }
}
