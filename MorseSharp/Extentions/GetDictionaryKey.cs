namespace MorseSharp.Extentions;


/// <summary>
/// Provides extension methods for dictionaries to retrieve keys by their associated values.
/// </summary>
internal static class GetDictionaryKey
{
    /// <summary>
    /// Retrieves the key associated with the specified value from a dictionary.
    /// </summary>
    /// <typeparam name="T">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="V">The type of values in the dictionary.</typeparam>
    /// <param name="dict">The dictionary to search for the key-value pair.</param>
    /// <param name="val">The value to look for in the dictionary.</param>
    /// <returns>The key associated with the specified value, or the default value of type <typeparamref name="T"/> if not found.</returns>
    public static T KeyByValue<T, V>(this Dictionary<T, V> dict, V val)
    {
        T key = default!;
        foreach (KeyValuePair<T, V> pair in dict)
        {
            if (EqualityComparer<V>.Default.Equals(pair.Value, val))
            {
                key = pair.Key;
                break;
            }
        }

        return key;
    }
}