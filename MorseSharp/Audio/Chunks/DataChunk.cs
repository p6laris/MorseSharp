using MorseSharp.Helpers;

namespace MorseSharp.Audio.Chunks;
/// <summary>
/// Represents a data chunk for WAVE files.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly ref struct ValueDataChunk
{

    private readonly Span<short> _chunkData;
    private readonly uint _chunkSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueDataChunk"/> struct with the provided data.
    /// </summary>
    /// <param name="data">The data to be stored in the chunk.</param>
    public ValueDataChunk(Span<short> data)
    {
        _chunkData = data;
        _chunkSize = (uint)(data.Length * 2);
    }

    public int Capacity => (2 * 4) + (_chunkData.Length * 2);
    /// <summary>
    /// Gets the size of the data chunk in bytes.
    /// </summary>
    /// <returns>The size of the data chunk in bytes.</returns>
    public uint GetChunkSize() => _chunkSize;

    /// <summary>
    /// Converts the data chunk to a byte array.
    /// </summary>
    /// <returns>A <see cref="Span{byte}"/> containing the data chunk as bytes.</returns>
    [SkipLocalsInit]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> ToBytes()
    {
        Span<byte> chunkId = [100, 97, 116, 97];

        using SpanOwner<byte> owner = SpanOwner<byte>.Allocate(Capacity);
        Span<byte> data = owner.Span;

        SpanByteWriter writer = new SpanByteWriter(ref data);

        writer.AddRange(chunkId);
        writer.AddRangeBit(_chunkSize);
        for (int i = 0; i < _chunkData.Length; i++)
        {
            writer.AddRangeBit(_chunkData[i]);
        }

        return owner.Span;
    }
}
