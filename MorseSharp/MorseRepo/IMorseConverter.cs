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
        /// <param name="Text">The <see cref="System.String"></see> to convert to morse code.</param>
        /// <returns><see cref="string"></see> of the morse result.</returns>
        string ConvertTextToMorse(string Text);
        /// <summary>
        /// Converts a morse message to english sentence
        /// </summary>
        /// <param name="Morse">The <see cref="string"/> of the morse message.</param>
        /// <returns><see cref="string"/>of the converted morse.</returns>
        string ConvertMorseToText(string Morse);
    }
}
