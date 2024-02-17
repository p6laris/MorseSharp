namespace MorseSharp.Exceptions
{
    [Serializable]
    public class SmallerCharSpeedException(int charSpeed, int wordSpeed) 
        : Exception($"The character speed must not be smaller than word speed: {charSpeed} < {wordSpeed}")
    {
        public int CharSpeed { get; } = charSpeed;
        public int WordSpeed { get; } = wordSpeed;
    }
}
