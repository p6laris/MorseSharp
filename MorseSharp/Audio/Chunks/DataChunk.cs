using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseSharp.Audio.Chunks
{
    internal class DataChunk : WaveChunk
    {
        public Memory<short> ChunkData { get; set; }

        public DataChunk(short[] data)
        {
            ChunkId = "data".ToCharArray();
            ChunkSize = (uint)(data.Length * 2);
            ChunkData = data;
        }

        public override Memory<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(Encoding.UTF8.GetBytes(ChunkId.ToArray()));
            bytes.AddRange(BitConverter.GetBytes(ChunkSize));

            
            foreach (short datum in ChunkData.ToArray())
                bytes.AddRange(BitConverter.GetBytes(datum));

            return bytes.ToArray<byte>();
        }
    }
}
