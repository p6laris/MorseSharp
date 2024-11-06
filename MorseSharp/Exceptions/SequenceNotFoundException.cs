namespace MorseSharp.Exceptions
{
    [Serializable]
    public class SequenceNotFoundException(ReadOnlySpan<char> sequence, Language language) 
        : Exception($"The Morse sequence '{sequence}' is not found in the {language} language.")
    {
        public string Sequence { get; } = sequence.ToString();
        
        public Language Language { get; } = language;
    }
}
