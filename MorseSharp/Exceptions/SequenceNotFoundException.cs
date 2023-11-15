namespace MorseSharp.Exceptions
{
    [Serializable]
    internal class SequenceNotFoundException : Exception
    {
        public SequenceNotFoundException() { }

        public SequenceNotFoundException(string sequence) :
            base($"The Morse sequence '{sequence}' is not found.")
        {

        }
        public SequenceNotFoundException(string sequence, Language language) :
           base($"The Morse sequence '{sequence}' is not found in the {language} language.")
        {

        }
    }
}
