namespace MorseSharp
{
    /// <summary>
    /// Defines all morse characters.
    /// </summary>
    internal static class MorseCharacters
    {
        /// <summary>
        /// Defines all morse characters in english language.
        /// </summary>
        /// <returns><see cref="System.Collections.Generic.Dictionary{TKey, TValue}"></see> of morse characters./></returns>
        public static Dictionary<char, string> GetMorseCharactersEnglish()
        {
            return new Dictionary<char, string>()
            {
                { 'A', "._" },
                { 'B', "_..." },
                { 'C', "_._." },
                { 'D', "_.."},
                { 'E', "." },
                { 'F', ".._." },
                { 'G', "__." },
                { 'H', "...." },
                { 'I', ".." },
                { 'J', ".___" },
                { 'K', "_._" },
                { 'L', "._.." },
                { 'M', "__" },
                { 'N', "_." },
                { 'O', "___" },
                { 'P', ".__." },
                { 'Q', "__._" },
                { 'R', "._." },
                { 'S', "..." },
                { 'T', "_" },
                { 'U', ".._" },
                { 'V', "..._" },
                { 'W', ".__"},
                { 'X', "_.._" },
                { 'Y', "_.__" },
                { 'Z', "__.." },
                //Numerics
                { '1', ".____" },
                { '2', "..__" },
                { '3', "...__" },
                { '4', "...._" },
                { '5', "....." },
                { '6', "_...." },
                { '7', "__..." },
                { '8', "___.." },
                { '9', "____." },
                { '0', "_____" },
                //Space
                {' ', "/" },
                //Punctuation
                {'.',"._._._" },
                {',',"__..__" },
                {'?',"..__.." },
                {';',"_._._." },
                {':',"___..." },
                {'/',"_.._."},
                {'\'',".____." },
                {'\"',"._.._." },
                //Special Characters
                {'_',"..__._"},
                {'+',"._._." },
                {'-',"_...._" },
                {'*',"_.._" },
                {'/',"___..." },
                {'=',"_..._" },
                {')',"_.__._" },
                {'(',"_.__." }
        };
        }
    }
}