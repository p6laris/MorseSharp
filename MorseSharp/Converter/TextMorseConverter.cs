using MorseSharp.MorseRepo;
using System.Text;

namespace MorseSharp.Converter
{

    /// <summary>
    /// Decode/encode's morse text.
    /// </summary>
    public class TextMorseConverter : IMorseConverter
    {
        private readonly StringBuilder? strBuilder;
        private readonly Dictionary<char, string> morseChar;
        private readonly Language language;
        private readonly Language nonLatin = Language.Kurdish | Language.Arabic;

        /// <summary>
        /// Create a new instance of type <see cref="TextMorseConverter"></see>.
        /// </summary>
        /// <param name="Language">The language convert from it to morse code.</param>
        public TextMorseConverter(Language Language)
        {
            language = Language;
            strBuilder = new StringBuilder();
            morseChar = MorseCharacters.GetLanguageCharacter(Language: language);
        }

        /// <summary>
        /// Converts the given string sentence to morse code.
        /// </summary>
        /// <param name="Text">The <see cref="System.String"></see> to convert to morse code.</param>
        /// <returns><see cref="string"></see> of the morse result.</returns>
        /// <exception cref="Exception">Throws if a character doesn't presented.</exception>
        /// <exception cref="ArgumentNullException">Throws if the string text was null.</exception>
        public Task<string> ConvertTextToMorse(string Text)
        {
            strBuilder!.Clear();

            if ((language & nonLatin) == 0)
                Text = Text.ToUpper();

            if (Text is not null)
            {
                for (int i = 0; i < Text.Length; i++)
                {
                    if (morseChar.ContainsKey(Text[i]))
                    {
                        strBuilder.Append(morseChar[Text[i]]);
                        strBuilder.Append(" ");
                    }

                    else
                    {
                        throw new KeyNotFoundException($"The {Text[i]} character is not presented.");
                    }

                }
                return Task.Run(() => strBuilder.ToString());
            }

            throw new ArgumentNullException(nameof(Text));
        }

        /// <summary>
        ///  Converts a morse message to english sentence
        /// </summary>
        /// <param name="Morse">The <see cref="string"/> of the morse message.</param>
        /// <returns><see cref="string"/>of the converted morse.</returns>
        /// <exception cref="Exception">Throws if the morse letter was not presented.</exception>
        /// <exception cref="ArgumentNullException">Throw if Morse param was null.</exception>
        public Task<string> ConvertMorseToText(string Morse)
        {
            strBuilder!.Clear();

            if (Morse is not null)
            {
                var words = Morse.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    if (!String.IsNullOrEmpty(words[i]))
                    {
                        if (words[i] != "/")
                        {
                            if (morseChar.Values.Contains(words[i]))
                            {
                                var word = morseChar.FirstOrDefault(x => x.Value == words[i]).Key;
                                strBuilder.Append(word);
                            }
                            else
                                throw new KeyNotFoundException($"The {Morse[i]} is not presented.");
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
