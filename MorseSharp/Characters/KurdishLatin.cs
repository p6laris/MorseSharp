namespace MorseSharp.Characters;

internal static class KurdishLatinCharacters
{
    internal static MorseTable256 KurdishLatinTable()
    {
        MorseTable256 table = new MorseTable256();

        // Kurdish Latin Alphabets
        table.Add('A', ".-");
        table.Add('B', "-...");
        table.Add('P', ".--.");
        table.Add('T', "-");
        table.Add('C', ".---");
        table.Add('Ç', "---.");
        table.Add('Ü', "....");
        table.Add('X', "-..-");
        table.Add('D', "-..");
        table.Add('R', "-.-");
        table.Add('Ř', ".-.");
        table.Add('Z', "--..");
        table.Add('J', "--.");
        table.Add('S', "...");
        table.Add('Ş', "----");
        table.Add('W', "---");
        table.Add('Y', "..--");
        table.Add('F', "..-.");
        table.Add('V', "..-..");
        table.Add('Ň', "...---");
        table.Add('K', "-.-..");
        table.Add('G', "--.-");
        table.Add('L', ".-..");
        table.Add('Ł', "...-");
        table.Add('M', "--");
        table.Add('N', "-.");
        table.Add('H', "-.-.");
        table.Add('E', ".");
        table.Add('U', ".--");
        table.Add('Û', ".--.--");
        table.Add('O', ".-.-");
        table.Add('Î', "..");
        table.Add('Ê', "..-");
        table.Add('I', "..-..-");

        // Numbers
        table.Add('1', ".----");
        table.Add('2', "..---");
        table.Add('3', "...--");
        table.Add('4', "....-");
        table.Add('5', ".....");
        table.Add('6', "-....");
        table.Add('7', "--...");
        table.Add('8', "---..");
        table.Add('9', "----.");
        table.Add('0', "-----");

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

    internal static MorseTableReverse256 KurdishLatinReversedTable()
    {
        MorseTableReverse256 reversed = new MorseTableReverse256();

        // Kurdish Latin Alphabets
        reversed.Add(".-", 'A');
        reversed.Add("-...", 'B');
        reversed.Add(".--.", 'P');
        reversed.Add("-", 'T');
        reversed.Add(".---", 'C');
        reversed.Add("---.", 'Ç');
        reversed.Add("....", 'Ü');
        reversed.Add("-..-", 'X');
        reversed.Add("-..", 'D');
        reversed.Add("-.-", 'R');
        reversed.Add(".-.", 'Ř');
        reversed.Add("--..", 'Z');
        reversed.Add("--.", 'J');
        reversed.Add("...", 'S');
        reversed.Add("----", 'Ş');
        reversed.Add("---", 'W');
        reversed.Add("..--", 'Y');
        reversed.Add("..-.", 'F');
        reversed.Add("..-..", 'V');
        reversed.Add("...---", 'Ň');
        reversed.Add("-.-..", 'K');
        reversed.Add("--.-", 'G');
        reversed.Add(".-..", 'L');
        reversed.Add("...-", 'Ł');
        reversed.Add("--", 'M');
        reversed.Add("-.", 'N');
        reversed.Add("-.-.", 'H');
        reversed.Add(".", 'E');
        reversed.Add(".--", 'U');
        reversed.Add(".--.--", 'Û');
        reversed.Add(".-.-", 'O');
        reversed.Add("..", 'Î');
        reversed.Add("..-", 'Ê');
        reversed.Add("..-..-", 'I');

        // Numbers
        reversed.Add(".----", '1');
        reversed.Add("..---", '2');
        reversed.Add("...--", '3');
        reversed.Add("....-", '4');
        reversed.Add(".....", '5');
        reversed.Add("-....", '6');
        reversed.Add("--...", '7');
        reversed.Add("---..", '8');
        reversed.Add("----.", '9');
        reversed.Add("-----", '0');

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
