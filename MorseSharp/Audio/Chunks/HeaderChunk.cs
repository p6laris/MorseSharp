using System.Text;

namespace MorseSharp.Audio.Chunks
{
    internal class HeaderChunk : WaveChunk
    {
        public ReadOnlyMemory<char> RiffType { get; set; }
        public FormatChunk FormatChunk { get; set; }
        public DataChunk DataChunk { get; set; }

        public HeaderChunk(FormatChunk formatChunk, DataChunk dataChunk)
        {
            ChunkId = "RIFF".ToCharArray();
            RiffType = "WAVE".ToCharArray();
            FormatChunk = formatChunk;
            DataChunk = dataChunk;
            ChunkSize = 36 + DataChunk.ChunkSize;
        }

        public override Memory<byte> ToBytes()
        {
            List<byte> bytes = new List<byte>();

            bytes.AddRange(Encoding.UTF8.GetBytes(ChunkId.ToArray() ));
            bytes.AddRange(BitConverter.GetBytes(ChunkSize));
            bytes.AddRange(Encoding.UTF8.GetBytes(RiffType.ToArray()));
            bytes.AddRange(FormatChunk.ToBytes().ToArray());
            bytes.AddRange(DataChunk.ToBytes().ToArray());

            return bytes.ToArray<byte>();
        }
    }
}
