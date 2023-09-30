namespace MorseSharp.Audio.Chunks
{
    internal abstract class WaveChunk
    {
        public ReadOnlyMemory<char> ChunkId { get; set; }
        public uint ChunkSize { get; set; }

        public abstract Memory<byte> ToBytes();
    }
}
