namespace MorseSharp.Extensions;


internal static class GetDictionaryKey
{
    public static char KeyByValue(this Dictionary<char, string> dict, string val)
    {
        if (string.IsNullOrEmpty(val))
            return '\0'; 

        foreach (KeyValuePair<char, string> pair in dict)
        {
            if (pair.Value == val)
            {
                return pair.Key;  
            }
        }

        return '\0';  
    }
}