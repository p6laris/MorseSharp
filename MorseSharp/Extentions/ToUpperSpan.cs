namespace MorseSharp.Extentions;

/// <summary>
/// Provides extension methods for converting characters to uppercase using pointers.
/// </summary>
internal static class ToUpperSpanExtention
{
    /// <summary>
    /// Converts characters in a span to uppercase using pointers.
    /// </summary>
    /// <param name="inputPtr">A pointer to the input character span.</param>
    /// <param name="destPtr">A pointer to the destination character span where the uppercase characters will be written.</param>
    /// <param name="length">The length of the character span.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="inputPtr"/> or <paramref name="destPtr"/> is null, or when <paramref name="length"/> is less than 0.</exception>
    public static unsafe void ToUpperWithPointers(char* inputPtr, char* destPtr, int length)
    {
        if (inputPtr == null || destPtr == null || length < 0) 
            throw new ArgumentNullException();

        for (int i = 0; i < length; i++)
        {
            char c = inputPtr[i];
            destPtr[i] = (c >= 'a' && c <= 'z') ? (char) (c - 32) : c;
        }
    }

}