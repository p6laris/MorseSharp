namespace MorseSharp.Characters;

internal static class KurdishCharacters
{
    internal static MorseTable256 KurdishTable()
    {
        MorseTable256 table = new MorseTable256();

        // Kurdish Alphabets
        table.Add('ا', ".-");
        table.Add('ب', "-...");
        table.Add('پ', ".--.");
        table.Add('ت', "-");
        table.Add('ج', ".---");
        table.Add('چ', "---.");
        table.Add('ح', "....");
        table.Add('خ', "-..-");
        table.Add('د', "-..");
        table.Add('ر', "-.-");
        table.Add('ڕ', ".-.");
        table.Add('ز', "--..");
        table.Add('ژ', "--.");
        table.Add('س', "...");
        table.Add('ش', "----");
        table.Add('ع', "---");
        table.Add('غ', "..--");
        table.Add('ف', "..-.");
        table.Add('ڤ', "..-..");
        table.Add('ق', "...---");
        table.Add('ک', "-.-..");
        table.Add('گ', "--.-");
        table.Add('ل', ".-..");
        table.Add('ڵ', "...-");
        table.Add('م', "--");
        table.Add('ن', "-.");
        table.Add('ه', "-.-.");
        table.Add('ە', ".");
        table.Add('و', ".--");
        table.Add('ۆ', ".-.-");
        table.Add('ی', "..");
        table.Add('ێ', "..-");
        table.Add('ئ', "..-..-");

        // Numbers (Arabic numerals)
        table.Add('٠', "-----");
        table.Add('١', ".----");
        table.Add('٢', "..---");
        table.Add('٣', "...--");
        table.Add('٤', "....-");
        table.Add('٥', ".....");
        table.Add('٦', "-....");
        table.Add('٧', "--...");
        table.Add('٨', "---..");
        table.Add('٩', "----.");

        // Space
        table.Add(' ', "/");

        // Punctuation
        table.Add('.', ".-.-.-.");
        table.Add('،', "--..--");
        table.Add('؟', "..--..");
        table.Add(':', "---...");
        table.Add('-', "-....-");
        table.Add('/', "-..-.");
        table.Add('\'', ".----.");
        table.Add('\"', ".-..-.");
        table.Add('؛', "-.-.-.");

        // Special Characters
        table.Add('_', "..--.-");
        table.Add('+', ".-.-.");
        table.Add('*', "-..-");
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

    internal static MorseTableReverse256 KurdishReversedTable()
    {
        MorseTableReverse256 reversed = new MorseTableReverse256();

        // Kurdish Alphabets
        reversed.Add(".-", 'ا');
        reversed.Add("-...", 'ب');
        reversed.Add(".--.", 'پ');
        reversed.Add("-", 'ت');
        reversed.Add(".---", 'ج');
        reversed.Add("---.", 'چ');
        reversed.Add("....", 'ح');
        reversed.Add("-..-", 'خ');
        reversed.Add("-..", 'د');
        reversed.Add("-.-", 'ر');
        reversed.Add(".-.", 'ڕ');
        reversed.Add("--..", 'ز');
        reversed.Add("--.", 'ژ');
        reversed.Add("----", 'ش');
        reversed.Add("---", 'ع');
        reversed.Add("..--", 'غ');
        reversed.Add("..-.", 'ف');
        reversed.Add("..-..", 'ڤ');
        reversed.Add("...---", 'ق');
        reversed.Add("-.-..", 'ک');
        reversed.Add("--.-", 'گ');
        reversed.Add(".-..", 'ل');
        reversed.Add("...-", 'ڵ');
        reversed.Add("--", 'م');
        reversed.Add("-.", 'ن');
        reversed.Add("-.-.", 'ه');
        reversed.Add(".", 'ە');
        reversed.Add(".--", 'و');
        reversed.Add(".-.-", 'ۆ');
        reversed.Add("..", 'ی');
        reversed.Add("..-", 'ێ');
        reversed.Add("..-..-", 'ئ');

        // Numbers
        reversed.Add("-----", '٠');
        reversed.Add(".----", '١');
        reversed.Add("..---", '٢');
        reversed.Add("...--", '٣');
        reversed.Add("....-", '٤');
        reversed.Add(".....", '٥');
        reversed.Add("-....", '٦');
        reversed.Add("--...", '٧');
        reversed.Add("---..", '٨');
        reversed.Add("----.", '٩');

        // Space
        reversed.Add("/", ' ');

        // Punctuation
        reversed.Add(".-.-.-.", '.');
        reversed.Add("--..--", '،');
        reversed.Add("..--..", '؟');
        reversed.Add("---...", ':');
        reversed.Add("-....-", '-');
        reversed.Add("-..-.", '/');
        reversed.Add(".----.", '\'');
        reversed.Add(".-..-.", '\"');
        reversed.Add("-.-.-.", '؛');

        // Special Characters
        reversed.Add("..--.-", '_');
        reversed.Add(".-.-.", '+');
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
