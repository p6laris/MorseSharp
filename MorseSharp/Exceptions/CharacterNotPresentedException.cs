namespace MorseSharp.Exceptions
{
    [Serializable]
    public class CharacterNotPresentedException : Exception
    {

        public CharacterNotPresentedException() { }
        public CharacterNotPresentedException(char word)
            : base($"The {word} character is not presented in the specified Language.")
        {

        }
        public CharacterNotPresentedException(char word, Language language)
            : base($"The {word} character is not presented in {Enum.GetName(language)} Language.")
        {

        }
    }
}
