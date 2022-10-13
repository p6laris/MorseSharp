using MorseSharp.MorseRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseSharp.Converter
{
    public class MorseTextConverter : IMorseTextConverter
    {
        /// <summary>
        /// <see cref="System.Text.StringBuilder"></see> object to append converted morse codes./>
        /// </summary>
        private StringBuilder strBuilder;

        /// <summary>
        /// Loads the morse characters.
        /// </summary>
        private Lazy<Dictionary<char, string>> morseEnglish;
        private Lazy<Dictionary<char, string>> morseKurdish;
        public MorseTextConverter()
        {
            morseEnglish = new Lazy<Dictionary<char, string>>(MorseCharacters.GetMorseCharactersEnglish);
            morseKurdish = new Lazy<Dictionary<char, string>>(MorseCharacters.GetMorseCharactersKurdish);
        }
       /// <summary>
       /// Converts a morse message to english sentence
       /// </summary>
       /// <param name="Morse">The <see cref="string"/> of the morse message.</param>
       /// <returns><see cref="string"/>of the converted morse.</returns>
       /// <exception cref="Exception">Throws if the morse letter was not presented.</exception>
       /// <exception cref="ArgumentNullException">Throw if Morse param was null.</exception>
        public Task<string> ConvertMorseToEnglish(string Morse)
        {
            strBuilder = new StringBuilder();

            if (Morse is not null)
            {
                var words = Morse.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    if (!String.IsNullOrEmpty(words[i]))
                    {
                        if (words[i] != "/")
                        {
                            if (morseEnglish.Value.Values.Contains(words[i]))
                            {
                                var word = morseEnglish.Value.FirstOrDefault(x => x.Value == words[i]).Key;
                                strBuilder.Append(word);
                            }
                            else
                                throw new Exception($"The {Morse[i]} is not presented.");
                        }
                        else
                            strBuilder.Append(' ');
                    }
                }

                return Task.Run(() => strBuilder.ToString());
            }
            throw new ArgumentNullException(nameof(Morse));
        }
        /// <summary>
        /// Converts a morse message to kurdish sentence
        /// </summary>
        /// <param name="Morse">The <see cref="string"/> of the morse message.</param>
        /// <returns><see cref="string"/>of the converted morse.</returns>
        /// <exception cref="Exception">Throws if the morse letter was not presented.</exception>
        /// <exception cref="ArgumentNullException">Throw if Morse param was null.</exception>
        public Task<string> ConvertMorseToKurdish(string Morse)
        {
            
            strBuilder = new StringBuilder();

            if (Morse is not null)
            {
                var words = Morse.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    if (!String.IsNullOrEmpty(words[i]))
                    {
                        if (words[i] != "/")
                        {
                            if (morseKurdish.Value.Values.Contains(words[i]))
                            {
                                var word = morseKurdish.Value.FirstOrDefault(x => x.Value == words[i]).Key;
                                strBuilder.Append(word);
                            }
                            else
                                throw new Exception($"The {Morse[i]} is not presented.");
                        }
                        else
                            strBuilder.Append(' ');
                    }
                }

                return Task.Run(() => strBuilder.ToString());
            }
            throw new ArgumentNullException(nameof(Morse));
        }
    }
}
