namespace MorseSharp.Exceptions
{
    [Serializable]
    public class SequenceNotFoundException(string sequence, Language language) 
        : Exception($"The Morse sequence '{sequence}' is not found in the {language} language.")
    {
        public string Sequence { get; } = (!string.IsNullOrEmpty(sequence))
            ? sequence
            : throw new ArgumentNullException(nameof(sequence));
        
        public Language Language { get; } = language;
    }
}
