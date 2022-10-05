namespace MorseSharp.MorseRepo
{
    /// <summary>
    /// Morse text converter abstraction.
    /// </summary>
    internal interface IMorseConverter
    {
        /// <summary>
        /// Converts a string sentence english to morse code.
        /// </summary>
        /// <param name="text">A <see cref="System.String"></see> of the sentence to convert to morse code./></param>
        /// <returns><see cref="System.Threading.Tasks.Task{string}"></see> of the morse code string./></returns>
        Task<string> ConvertToMorseEnglish(string text);
        /// <summary>
        /// Converts a string sentence kurdish to morse code.
        /// </summary>
        /// <param name="text">A <see cref="System.String"></see> of the sentence to convert to morse code./></param>
        /// <returns><see cref="System.Threading.Tasks.Task{string}"></see> of the morse code string./></returns>
        Task<string> ConvertToMorseKurdish(string text);
    }
}
