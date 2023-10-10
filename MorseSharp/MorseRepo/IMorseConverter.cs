namespace MorseSharp.MorseRepo
{
    /// <summary>
    /// Morse text converter abstraction.
    /// </summary>
    public interface IMorseConverter
    {
        /// <summary>
        /// Converts the given string sentence to morse code.
        /// </summary>
        /// <param name="Text">The text to convert to morse code.</param>
        /// <returns>A string containing the morse code result.</returns>
        string ConvertTextToMorse(string Text);

        /// <summary>
        /// Converts a morse message to an English sentence.
        /// </summary>
        /// <param name="Morse">The morse message to convert.</param>
        /// <returns>A string containing the converted morse message.</returns>
        string ConvertMorseToText(string Morse);
    }
}
