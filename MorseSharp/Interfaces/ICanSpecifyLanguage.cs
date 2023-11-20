namespace MorseSharp.Interfaces
{
    /// <summary>
    /// Represents an interface that allows specifying a target language for conversion.
    /// </summary>
    public interface ICanSpecifyLanguage
    {
        /// <summary>
        /// Specifies the target language for conversion.
        /// </summary>
        /// <param name="language">The target language to be set.</param>
        /// <returns>An object that allows setting conversion options for the specified language.</returns>
        ICanSetConversionOption ForLanguage(Language language);
    }
}