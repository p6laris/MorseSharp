namespace MorseSharp.Exceptions
{
    [Serializable]
    public class SmallerCharSpeedException : Exception
    {
        public SmallerCharSpeedException() { }

        public SmallerCharSpeedException(string message) : base(message)
        {

        }
        public SmallerCharSpeedException(int charSpeed, int wordSpeed)
            : base($"The character speed must not be smaller than word speed: {charSpeed} < {wordSpeed}")
        {

        }
    }
}
