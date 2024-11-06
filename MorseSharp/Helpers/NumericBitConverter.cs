namespace MorseSharp.Helpers
{
    internal static class NumericBitConverter
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBytes(short value, Span<byte> destination)
        {
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBytes(int value, Span<byte> destination)
        {
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBytes(uint value, Span<byte> destination)
        {
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void GetBytes(long value, Span<byte> destination)
        {
            Unsafe.WriteUnaligned(ref MemoryMarshal.GetReference(destination), value);
        }
    }
}
