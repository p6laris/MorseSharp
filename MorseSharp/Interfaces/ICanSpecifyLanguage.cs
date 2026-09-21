namespace MorseSharp.Interfaces;

/// <summary>
/// First step of the fluent chain: choose the alphabet.
/// </summary>
public interface ICanSpecifyLanguage
{
    /// <summary>
    /// Selects the alphabet used for the rest of the chain.
    /// </summary>
    /// <param name="language">One of the supported languages.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="language"/> is not a defined value.</exception>
    ICanSetConversionOption ForLanguage(Language language);
}
