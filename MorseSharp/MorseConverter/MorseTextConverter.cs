using MorseSharp.MorseRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseSharp.MorseConverter
{
    public class MorseTextConverter : IMorseConverter
    {
        /// <summary>
        /// <see cref="System.Text.StringBuilder"></see> object to append converted morse codes./>
        /// </summary>
        private StringBuilder? strBuilder;

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
        /// Converts the given string sentence to morse code.
        /// </summary>
        /// <param name="text">The <see cref="System.String"></see> to convert to morse code.</param>
        /// <returns><see cref="Task{string}"></see> of the morse result.</returns>
        /// <exception cref="Exception">Throws if a character doesn't presented.</exception>
        /// <exception cref="ArgumentNullException">Throws if the string text was null.</exception>
        public Task<string> ConvertToMorseEnglish(string text)
        {
            strBuilder = new StringBuilder();
            text = text.ToUpper();

            if(text is not null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (morseEnglish.Value.ContainsKey(text[i]))
                    {  
                        strBuilder.Append(morseEnglish.Value[text[i]].AsSpan());
                        strBuilder.Append(" ");
                    }
                        
                    else
                    {
                        throw new Exception($"The {text[i]} character is not presented.");
                    }
                        
                }
                return Task.Run(() => strBuilder.ToString());
            }
            
            throw new ArgumentNullException(nameof(text));
        }

        public Task<string> ConvertToMorseKurdish(string text)
        {
            strBuilder = new StringBuilder();
            text = text.ToUpper();

            if (text is not null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (morseKurdish.Value.ContainsKey(text[i]))
                    {
                        strBuilder.Append(morseKurdish.Value[text[i]].AsSpan());
                        strBuilder.Append(" ");
                    }

                    else
                    {
                        throw new Exception($"The {text[i]} character is not presented.");
                    }

                }
                return Task.Run(() => strBuilder.ToString());
            }
            throw new ArgumentNullException(nameof(text));
        }
    }
}
