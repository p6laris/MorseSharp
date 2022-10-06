using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseSharp.Audio.Chunks
{
    internal abstract class WaveChunk
    {
        public ReadOnlyMemory<char> ChunkId { get; set; }
        public uint ChunkSize { get; set; }

        public abstract Memory<byte> ToBytes();
    }
}
