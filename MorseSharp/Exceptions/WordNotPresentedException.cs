
namespace MorseSharp.Exceptions
{
    [Serializable]
    internal class WordNotPresentedException : Exception
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
