using MorseSharp.MorseRepo;
using System.Text;

namespace MorseSharp.Converter
{
    /// <summary>
    /// Decode/encode's morse text.
    /// </summary>
    public class TextMorseConverter : IMorseConverter
    {
        private readonly StringBuilder strBuilder;
        private readonly Dictionary<char, string> morseChar;
        private readonly Language language;
        private readonly Language nonLatin = Language.Kurdish | Language.Arabic;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMorseConverter"/> class.
        /// </summary>
        /// <param name="Language">The language to convert from to morse code.</param>
        public TextMorseConverter(Language Language)
        {
            language = Language;
            strBuilder = new StringBuilder();
            morseChar = MorseCharacters.GetLanguageCharacter(Language: language);
        }

        /// <summary>
        /// Converts the given string sentence to morse code.
        /// </summary>
        /// <param name="Text">The text to convert to morse code.</param>
        /// <returns>A string containing the morse code result.</returns>
        /// <exception cref="Exception">Thrown if a character is not presented in the character mapping.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the input string is null.</exception>
        public string ConvertTextToMorse(string Text)
        {

            if (Text is not null)
            {
                strBuilder.Clear();
                if ((language & nonLatin) == 0)
                    Text = Text.ToUpper();


                for (int i = 0; i < Text.Length; i++)
                {
                    if (morseChar.TryGetValue(Text[i], out string val))
                    {

                        // Check if it's not the last character before appending a space
                        if (i < Text.Length - 1)
                        {
                            strBuilder.Append(val).Append(' ');
                        }
                        else
                            strBuilder.Append(val);
                    }

                    else
                    {
                        throw new KeyNotFoundException($"The {Text[i]} character is not presented.");
                    }

                }
                return strBuilder.ToString();
            }

            throw new ArgumentNullException(nameof(Text));
        }

        /// <summary>
        /// Converts a morse message to an English sentence.
        /// </summary>
        /// <param name="Morse">The morse message to convert.</param>
        /// <returns>A string containing the converted morse message.</returns>
        /// <exception cref="Exception">Thrown if a morse letter is not presented in the character mapping.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the input morse message is null.</exception>
        public string ConvertMorseToText(string Morse)
        {

            if (Morse is not null)
            {
                strBuilder.Clear();
                var words = Morse.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    if (!String.IsNullOrEmpty(words[i]))
                    {
                        if (words[i] != "/")
                        {
                            if (morseChar.Values.Contains(words[i]))
                            {
                                var word = morseChar.First(x => x.Value == words[i]).Key;
                                strBuilder.Append(word);
                            }
                            else
                                throw new KeyNotFoundException($"The {words[i]} is not presented.");
                        }
                        else
                            strBuilder.Append(' ');
                    }
                }

                return strBuilder.ToString();
            }
            throw new ArgumentNullException(nameof(Morse));
        }
    }
}
