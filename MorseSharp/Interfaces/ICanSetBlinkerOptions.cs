namespace MorseSharp.Interfaces
{
    public interface ICanSetBlinkerOptions
    {
        ICanConvertToLight SetBlinkerOptions(int charSpeed, int wordSpeed);
    }
}
