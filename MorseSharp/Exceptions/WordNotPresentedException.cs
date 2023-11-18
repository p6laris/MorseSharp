
namespace MorseSharp.Exceptions
{
    [Serializable]
    public class WordNotPresentedException : Exception
    {

        public WordNotPresentedException() { }
        public WordNotPresentedException(char word)
            : base($"The {word} word is not presented in the specified Language.")
        {

        }
        public WordNotPresentedException(char word, Language language)
            : base($"The {word} word is not presented in {Enum.GetName(language)} Language.")
        {

        }
    }
}
