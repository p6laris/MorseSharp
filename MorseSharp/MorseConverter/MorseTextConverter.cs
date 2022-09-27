using MorseSharp.MorseRepo;
using Serilog.Core;
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
        private Lazy<Dictionary<char, string>> morse;
        public MorseTextConverter()
        {
            morse = new Lazy<Dictionary<char, string>>(MorseCharacters.GetMorseCharactersEnglish);
         
        }
        public Task<string> ConvertToMorse(string text)
        {
            strBuilder = new StringBuilder();
            text = text.ToUpper();

            if(text is not null)
            {
                for (int i = 0; i < text.Length; i++)
                {
                    if (morse.Value.ContainsKey(text[i]))
                        strBuilder.Append(morse.Value[text[i]].AsSpan());
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
