namespace MorseSharp.Helpers;

/// <summary>
/// Provides static methods to convert numeric values to little-endian byte representations.
/// </summary>
internal static class NumericBitConverter
{
    /// <summary>
    /// Converts a 16-bit signed integer to a little-endian byte representation.
    /// </summary>
    /// <param name="value">The 16-bit signed integer to convert.</param>
    /// <param name="destination">The destination span where the byte representation will be written.</param>
    /// <exception cref="ArgumentException">Thrown when the destination span does not have a length of at least 2 bytes.</exception>
    public static void GetBytes(short value, Span<byte> destination)
    {
        if (destination.Length < 2)
        {
            throw new ArgumentException("Destination span must have a length of at least 2 bytes.",
                nameof(destination));
        }

        unsafe
        {
            fixed (byte* bPtr = &destination.GetPinnableReference())
            {
                short* shPtr = (short*)bPtr;
                *shPtr = value;
            }
        }
    }
    /// <summary>
    /// Converts a 32-bit signed integer to a little-endian byte representation.
    /// </summary>
    /// <param name="value">The 32-bit signed integer to convert.</param>
    /// <param name="destination">The destination span where the byte representation will be written.</param>
    /// <exception cref="ArgumentException">Thrown when the destination span does not have a length of at least 4 bytes.</exception>
    public static void GetBytes(int value, Span<byte> destination)
    {
        if (destination.Length < 4)
        {
            throw new ArgumentException("Destination span must have a length of at least 4 bytes.",
                nameof(destination));
        }

        unsafe
        {
            fixed (byte* bPtr = &destination.GetPinnableReference())
            {
                int* intPtr = (int*)bPtr;
                *intPtr = value;
            }
        }
    }
    /// <summary>
    /// Converts a 32-bit unsigned integer to a little-endian byte representation.
    /// </summary>
    /// <param name="value">The 32-bit unsigned integer to convert.</param>
    /// <param name="destination">The destination span where the byte representation will be written.</param>
    /// <exception cref="ArgumentException">Thrown when the destination span does not have a length of at least 4 bytes.</exception>
    public static void GetBytes(uint value, Span<byte> destination)
    {
        if (destination.Length < 4)
        {
            throw new ArgumentException("Destination span must have a length of at least 4 bytes.",
                nameof(destination));
        }

        unsafe
        {
            fixed (byte* bPtr = &destination.GetPinnableReference())
            {
                uint* uintPtr = (uint*)bPtr;
                *uintPtr = value;
            }
        }
    }

    /// <summary>
    /// Converts a 64-bit signed integer to a little-endian byte representation.
    /// </summary>
    /// <param name="value">The 64-bit signed integer to convert.</param>
    /// <param name="destination">The destination span where the byte representation will be written.</param>
    /// <exception cref="ArgumentException">Thrown when the destination span does not have a length of at least 8 bytes.</exception>
    public static void GetBytes(long value, Span<byte> destination)
    {
        if (destination.Length < 8)
        {
            throw new ArgumentException("Destination span must have a length of at least 8 bytes.",
                nameof(destination));
        }

        unsafe
        {
            fixed (byte* bPtr = &destination.GetPinnableReference())
            {
                long* longPtr = (long*)bPtr;
                *longPtr = value;
            }
        }
    }
}