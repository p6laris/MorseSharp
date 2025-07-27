namespace MorseSharp;

/// <summary>
/// Represents a Morse entry with a key, value, and occupancy status.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TVal">The type of the value.</typeparam>
public struct MorseEntry<TKey, TVal>
{
    public TKey Key;
    public TVal Value;
    public bool IsOccupied;
}

/// <summary>
/// Represents a fixed-size buffer of 256 Morse entries.
/// </summary>
/// <typeparam name="TKey">The type of the key.</typeparam>
/// <typeparam name="TVal">The type of the value.</typeparam>
[InlineArray(256)]
public struct MorseEntryBuffer256<TKey, TVal>
{
    MorseEntry<TKey, TVal> pair;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<MorseEntry<TKey, TVal>> AsSpan() => MemoryMarshal.CreateSpan(ref pair, 256);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<MorseEntry<TKey, TVal>> AsReadOnlySpan()
      => MemoryMarshal.CreateReadOnlySpan(ref pair, 256);
}

/// <summary>
/// Represents a Morse code lookup table using a fixed-size hash table of 256 slots.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct MorseTable256
{
    private MorseEntryBuffer256<char, string> _buffer;
    private const int Capacity = 256;
    private const int Mask = Capacity - 1;
    private const int Prime = 31;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Hash(char key)
    {
        uint h = (uint)key;
        h ^= h >> 4;
        h *= 0x27d4eb2d;
        return (int)(h & Mask);
    }

    internal void Add(char key, string value)
    {
        key = char.ToUpperInvariant(key);

        int index = Hash(key);
        var buffer = _buffer.AsSpan();

        for (int i = 0; i < Capacity; i++)
        {
            ref var entry = ref buffer[(index + i) & Mask];
            if (entry.IsOccupied)
            {
                if (entry.Key == key)
                    throw new ArgumentException($"An element with the same key '{key}' already exists.");
            }
            else
            {
                entry.Key = key;
                entry.Value = value;
                entry.IsOccupied = true;
                return;
            }
        }
        throw new InvalidOperationException("Hash table is full.");
    }

    /// <summary>
    /// Tries to get the Morse value associated with the given character key.
    /// </summary>
    /// <param name="key">The character key to look up.</param>
    /// <param name="value">When this method returns, contains the value associated with the key, if found.</param>
    /// <returns><c>true</c> if the key was found; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(char key, out ReadOnlySpan<char> value)
    {
        key = char.ToUpperInvariant(key);

        int index = Hash(key);
        var buffer = _buffer.AsReadOnlySpan();

        for (int i = 0; i < Capacity; i++)
        {
            var entry = buffer[(index + i) & Mask];
            if (!entry.IsOccupied)
                break;
            if (entry.Key == key)
            {
                value = entry.Value.AsSpan();
                return true;
            }
        }
        value = default;
        return false;
    }
}

/// <summary>
/// Represents a reverse lookup Morse code table from Morse strings to characters.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct MorseTableReverse256
{
    private MorseEntryBuffer256<string, char> _buffer;
    private const int Capacity = 256;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(string key, char value)
    {
        int index = ComputeHash(key);
        var buffer = _buffer.AsSpan();

        for (int i = 0; i < Capacity; i++)
        {
            int slot = (index + i) & (Capacity - 1);
            ref var entry = ref buffer[slot];

            if (entry.IsOccupied)
            {
                if (string.Equals(entry.Key, key, StringComparison.Ordinal))
                    throw new ArgumentException($"An element with the same key \"{key}\" value: {entry.Value} with {value} already exists.");
            }
            else
            {
                entry.Key = key;
                entry.Value = value;
                entry.IsOccupied = true;
                return;
            }
        }

        throw new InvalidOperationException("Reverse hash table is full.");
    }

    /// <summary>
    /// Tries to get the character value associated with the given Morse code.
    /// </summary>
    /// <param name="key">The Morse code sequence to look up.</param>
    /// <param name="value">When this method returns, contains the character associated with the key, if found.</param>
    /// <returns><c>true</c> if the Morse string was found; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetValue(ReadOnlySpan<char> key, out char value)
    {
        int index = ComputeHash(key);
        var buffer = _buffer.AsReadOnlySpan();

        for (int i = 0; i < Capacity; i++)
        {
            int slot = (index + i) & (Capacity - 1);
            var entry = buffer[slot];

            if (!entry.IsOccupied)
                break;

            if (entry.Key.AsSpan().SequenceEqual(key))
            {
                value = entry.Value;
                return true;
            }
        }

        value = '\0';
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int ComputeHash(ReadOnlySpan<char> s)
    {
        unchecked
        {
            const int fnvPrime = 16777619;
            int hash = (int)2166136261;

            for (int i = 0; i < s.Length; i++)
                hash = (hash ^ s[i]) * fnvPrime;

            return hash & 0x7FFFFFFF;
        }
    }

}
