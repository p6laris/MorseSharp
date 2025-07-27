namespace MorseSharp
{
    /*
    All characters except Kurdish, KurdishLatin
    Russian, sourced from https://morsedecoder.com/.
    Please if you notice any incorrectness in the characters feel free to
    Submit an issue in this repository https://github.com/p6laris/MorseSharp.
    */

    /// <summary>
    /// Defines all morse characters.
    /// </summary>
    public static class MorseCharacters
    {

        /// <summary>
        /// Gets a collection of a morse code characters for the given language.
        /// </summary>
        /// <param name="Language">The <see cref="MorseSharp.Language"/> </param>
        /// <returns><see cref="MorseTable256"></see> of morse characters.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static MorseTable256 GetLanguageCharacter(Language language) => language switch
        {
            Language.English => EnglishCharacters.EnglishTable(),
            Language.Kurdish => KurdishCharacters.KurdishTable(),
            Language.KurdishLatin => KurdishLatinCharacters.KurdishLatinTable(),
            Language.Arabic => ArabicCharacters.ArabicTable(),
            Language.Deutsch => DeutschCharacters.DeutschTable(),
            Language.Spanish => SpanishCharacters.SpanishTable(),
            Language.French => FrenchCharacters.FrenchTable(),
            Language.Italian => ItalianCharacters.ItalianTable(),
            Language.Japanese => JapaneseCharacters.JapaneseTable(),
            Language.Portugues => PortuguesCharacters.PortuguesTable(),
            Language.Russian => RussianCharacters.RussianTable(),
            _ => throw new ArgumentOutOfRangeException(nameof(language))
        };

        /// <summary>
        /// Gets a reversed collection of a morse code characters for the given language.
        /// </summary>
        /// <param name="Language">The <see cref="MorseSharp.Language"/> </param>
        /// <returns><see cref="MorseTableReverse256"></see> of morse characters.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static MorseTableReverse256 GetLanguageCharacterReversed(Language language) => language switch
        {
            Language.English => EnglishCharacters.EnglishReversedTable(),
            Language.Kurdish => KurdishCharacters.KurdishReversedTable(),
            Language.KurdishLatin => KurdishLatinCharacters.KurdishLatinReversedTable(),
            Language.Arabic => ArabicCharacters.ArabicReversedTable(),
            Language.Deutsch => DeutschCharacters.DeutschReversedTable(),
            Language.Spanish => SpanishCharacters.SpanishReversedTable(),
            Language.French => FrenchCharacters.FrenchReversedTable(),
            Language.Italian => ItalianCharacters.ItalianReversedTable(),
            Language.Japanese => JapaneseCharacters.JapaneseReversedTable(),
            Language.Portugues => PortuguesCharacters.PortuguesReversedTable(),
            Language.Russian => RussianCharacters.RussianReversedTable(),
            _ => throw new ArgumentOutOfRangeException(nameof(language))
        };
    }
}
