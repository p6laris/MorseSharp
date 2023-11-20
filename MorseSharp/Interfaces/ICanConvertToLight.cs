namespace MorseSharp.Interfaces
{
    public interface ICanConvertToLight
    {
        Task DoBlinks(Action<bool> blinkerAction);
    }
}
