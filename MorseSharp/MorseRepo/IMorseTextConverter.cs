using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseSharp.MorseRepo
{
    internal interface IMorseTextConverter
    {
        /// <summary>
        /// Converts a morse code to a kurdish sentence.
        /// </summary>
        /// <param name="Morse">A <see cref="System.String"></see> of the morse code to convert to sentence./></param>
        /// <returns><see cref="string"></see> of the kurdish sentence./></returns>
        Task<string> ConvertMorseToKurdish(string Morse);
        /// <summary>
        /// Converts a morse code to a english sentence.
        /// </summary>
        /// <param name="Morse">A <see cref="System.String"></see> of the morse code to convert to sentence./></param>
        /// <returns><see cref="string"></see> of the kurdish sentence./></returns>
        Task<string> ConvertMorseToEnglish(string Morse);
    }
}
