namespace MorseSharp.Helpers
{
    /// <summary>
    /// A helper struct for writing spans of bytes efficiently.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal ref struct SpanByteWriter
    {
        private int offset;
        private Span<byte> buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanByteWriter"/> struct with the provided buffer.
        /// </summary>
        /// <param name="buffer">The target buffer for writing bytes.</param>
        public SpanByteWriter(ref Span<byte> buffer)
        {
            this.buffer = buffer;
            offset = 0;
        }

        /// <summary>
        /// Adds a range of bytes from a source span to the target buffer.
        /// </summary>
        /// <param name="source">The source span containing the bytes to be added.</param>
        /// <exception cref="ArgumentException">Thrown when there is not enough space in the buffer to add the source data.</exception>
        public void AddRange(Span<byte> source)
        {
            if (offset + source.Length > buffer.Length)
                throw new ArgumentException("Not enough space in the buffer to add the source data.");

            source.CopyTo(buffer.Slice(offset));
            offset += source.Length;
        }

        /// <summary>
        /// Adds a range of bytes from a 32-bit unsigned integer (value) to the target buffer.
        /// </summary>
        /// <param name="value">The 32-bit unsigned integer value to be added as bytes.</param>
        /// <returns><c>true</c> if there is enough space in the buffer for the operation; otherwise, <c>false</c>.</returns>
        [SkipLocalsInit]
        public bool AddRangeBit(uint value)
        {
            Span<byte> bytes = stackalloc byte[4];
            NumericBitConverter.GetBytes(value, bytes);

            if (offset + bytes.Length > buffer.Length)
            {
                return false; // Not enough space in the buffer.
            }

            bytes.TryCopyTo(buffer.Slice(offset));
            offset += bytes.Length;
            return true;
        }

        /// <summary>
        /// Adds a range of bytes from a 16-bit signed integer (value) to the target buffer.
        /// </summary>
        /// <param name="value">The 16-bit signed integer value to be added as bytes.</param>
        /// <returns><c>true</c> if there is enough space in the buffer for the operation; otherwise, <c>false</c>.</returns>
        [SkipLocalsInit]
        public bool AddRangeBit(short value)
        {
            Span<byte> bytes = stackalloc byte[2];
            NumericBitConverter.GetBytes(value, bytes);

            if (offset + bytes.Length > buffer.Length)
                return false; // Not enough space in the buffer.

            bytes.TryCopyTo(buffer.Slice(offset));
            offset += bytes.Length;
            return true;
        }
    }
}
