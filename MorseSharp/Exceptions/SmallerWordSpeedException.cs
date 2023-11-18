
namespace MorseSharp.Exceptions
{
    [Serializable]
    public class SmallerWordSpeedException : Exception
    {
        public SmallerWordSpeedException() { }

        public SmallerWordSpeedException(string message) : base(message)
        {

        }
        public SmallerWordSpeedException(int charSpeed, int wordSpeed)
            : base($"The character speed must not be smaller than word speed: {charSpeed} < {wordSpeed}")
        {

        }
    }
}
