namespace MorseSharp.Characters;

internal static class ArabicCharacters
{
    internal static MorseTable256 ArabicTable()
    {
        MorseTable256 table = new MorseTable256();

        // Arabic Alphabet
        table.Add('ا', ".-");
        table.Add('ب', "-...");
        table.Add('ت', "-");
        table.Add('ث', "-.-.");
        table.Add('ج', ".---");
        table.Add('ح', "....");
        table.Add('خ', "---");
        table.Add('د', "-..");
        table.Add('ذ', "--..");
        table.Add('ر', ".-.");
        table.Add('ز', "---.");
        table.Add('س', "...");
        table.Add('ش', "----");
        table.Add('ص', "-..-");
        table.Add('ض', "...-");
        table.Add('ط', "..-");
        table.Add('ظ', "-.--");
        table.Add('ع', ".-.-");
        table.Add('غ', "--.");
        table.Add('ف', "..-.");
        table.Add('ق', "--.-");
        table.Add('ك', "-.-");
        table.Add('ل', ".-..");
        table.Add('م', "--");
        table.Add('ن', "-.");
        table.Add('ه', "..-..");
        table.Add('و', ".--");
        table.Add('ي', "..");
        table.Add('ء', ".");

        // Numbers
        table.Add('١', ".----");
        table.Add('٢', "..---");
        table.Add('٣', "...--");
        table.Add('٤', "....-");
        table.Add('٥', ".....");
        table.Add('٦', "-....");
        table.Add('٧', "--...");
        table.Add('٨', "---..");
        table.Add('٩', "----.");
        table.Add('٠', "-----");

        // Space
        table.Add(' ', "/");

        // Punctuation
        table.Add('.', ".-.-.-");
        table.Add('،', "--..--");
        table.Add('؟', "..--..");
        table.Add('؛', "-.-.-.");
        table.Add(':', "---...");
        table.Add('/', "-..-.");
        table.Add('‘', ".----.");
        table.Add('\"', ".-..-.");

        // Special Characters
        table.Add('_', "..--.-");
        table.Add('+', ".-.-.");
        table.Add('-', "-....-");
        table.Add('=', "-...-");
        table.Add(')', "-.--.-");
        table.Add('(', "-.--.");
        table.Add('$', "...-..-");
        table.Add('¿', "..-.-");
        table.Add('¡', "--...-");
        table.Add('&', ".-...");
        table.Add('@', ".--.-.");
        table.Add('!', "-.-.--");

        return table;
    }

    internal static MorseTableReverse256 ArabicReversedTable()
    {
        MorseTableReverse256 reversed = new MorseTableReverse256();

        // Arabic Alphabet
        reversed.Add(".-", 'ا');
        reversed.Add("-...", 'ب');
        reversed.Add("-", 'ت');
        reversed.Add("-.-.", 'ث');
        reversed.Add(".---", 'ج');
        reversed.Add("....", 'ح');
        reversed.Add("---", 'خ');
        reversed.Add("-..", 'د');
        reversed.Add("--..", 'ذ');
        reversed.Add(".-.", 'ر');
        reversed.Add("---.", 'ز');
        reversed.Add("...", 'س');
        reversed.Add("----", 'ش');
        reversed.Add("-..-", 'ص');
        reversed.Add("...-", 'ض');
        reversed.Add("..-", 'ط');
        reversed.Add("-.--", 'ظ');
        reversed.Add(".-.-", 'ع');
        reversed.Add("--.", 'غ');
        reversed.Add("..-.", 'ف');
        reversed.Add("--.-", 'ق');
        reversed.Add("-.-", 'ك');
        reversed.Add(".-..", 'ل');
        reversed.Add("--", 'م');
        reversed.Add("-.", 'ن');
        reversed.Add("..-..", 'ه');
        reversed.Add(".--", 'و');
        reversed.Add("..", 'ي');
        reversed.Add(".", 'ء');

        // Numbers
        reversed.Add(".----", '١');
        reversed.Add("..---", '٢');
        reversed.Add("...--", '٣');
        reversed.Add("....-", '٤');
        reversed.Add(".....", '٥');
        reversed.Add("-....", '٦');
        reversed.Add("--...", '٧');
        reversed.Add("---..", '٨');
        reversed.Add("----.", '٩');
        reversed.Add("-----", '٠');

        // Space
        reversed.Add("/", ' ');

        // Punctuation
        reversed.Add(".-.-.-", '.');
        reversed.Add("--..--", '،');
        reversed.Add("..--..", '؟');
        reversed.Add("-.-.-.", '؛');
        reversed.Add("---...", ':');
        reversed.Add("-..-.", '/');
        reversed.Add(".----.", '‘');
        reversed.Add(".-..-.", '\"');

        // Special Characters
        reversed.Add("..--.-", '_');
        reversed.Add(".-.-.", '+');
        reversed.Add("-....-", '-');
        reversed.Add("-...-", '=');
        reversed.Add("-.--.-", ')');
        reversed.Add("-.--.", '(');
        reversed.Add("...-..-", '$');
        reversed.Add("..-.-", '¿');
        reversed.Add("--...-", '¡');
        reversed.Add(".-...", '&');
        reversed.Add(".--.-.", '@');
        reversed.Add("-.-.--", '!');

        return reversed;
    }
}
