namespace MorseSharp.Interfaces;

/// <summary>
/// First step of the fluent chain: choose the alphabet.
/// </summary>
public interface ICanSpecifyLanguage
{
    /// <summary>
    /// Selects one of the built-in languages for the rest of the chain.
    /// </summary>
    /// <param name="language">One of the supported languages.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="language"/> is not a defined value.</exception>
    ICanSetConversionOption ForLanguage(Language language);

    /// <summary>
    /// Selects a custom alphabet for the rest of the chain.
    /// </summary>
    /// <param name="alphabet">An alphabet produced by <see cref="MorseAlphabetBuilder"/>.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="alphabet"/> is <c>null</c>.</exception>
    ICanSetConversionOption ForAlphabet(MorseAlphabet alphabet);
}
