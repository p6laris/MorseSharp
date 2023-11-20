namespace MorseSharp.Extentions
{
    /// <summary>
    /// Provides extension methods for counting occurrences of a character in a character span.
    /// </summary>
    internal static class StringCounter
    {
        /// <summary>
        /// Counts the occurrences of a specified character within a character span.
        /// </summary>
        /// <param name="input">A pointer to the character span to be searched.</param>
        /// <param name="targetCharacter">The character to count within the span.</param>
        /// <returns>The number of times the target character appears within the span.</returns>
        public static unsafe int CountOccurrences(char* input, char targetCharacter)
        {
            int count = 0;
            while (*input != '\0')
            {
                if (*input == targetCharacter)
                {
                    count++;
                }
                input++;
            }
            return count;
        }
    }
}