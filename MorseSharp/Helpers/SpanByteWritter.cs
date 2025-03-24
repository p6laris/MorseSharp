namespace MorseSharp.Helpers
{
    [StructLayout(LayoutKind.Sequential)]
    internal ref struct SpanByteWriter
    {
        private int offset;
        private Span<byte> buffer;

        public SpanByteWriter(ref Span<byte> buffer)
        {
            this.buffer = buffer;
            offset = 0;
        }

        public void AddRange(ReadOnlySpan<byte> source)
        {
            EnsureSpace(source.Length);
            source.CopyTo(buffer.Slice(offset));
            offset += source.Length;
        }

        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRangeBit(uint value)
        {
            EnsureSpace(sizeof(uint));
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer.Slice(offset)), value);
            offset += sizeof(uint);
        }

        [SkipLocalsInit]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRangeBit(short value)
        {
            EnsureSpace(sizeof(short));
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(buffer.Slice(offset)), value);
            offset += sizeof(short);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureSpace(int requiredBytes)
        {
            if (offset + requiredBytes > buffer.Length)
                throw new ArgumentException("Not enough space in the buffer.");
        }
    }
}
