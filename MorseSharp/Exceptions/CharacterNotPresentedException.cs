namespace MorseSharp.Exceptions
{
    [Serializable]
    public class CharacterNotPresentedException(char character, Language language)
        : Exception($"The {character} character is not presented in {Enum.GetName(language)} Language.")
    {
        public char Character { get; } = character;
        public Language Language { get; } = language;
    }
}
