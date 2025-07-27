namespace MorseSharp.Characters;

internal static class FrenchCharacters
{
    internal static MorseTable256 FrenchTable()
    {
        var table = new MorseTable256();

        // Alphabets
        table.Add('A', ".-");
        table.Add('B', "-...");
        table.Add('C', "-.-.");
        table.Add('D', "-..");
        table.Add('E', ".");
        table.Add('F', "..-.");
        table.Add('G', "--.");
        table.Add('H', "....");
        table.Add('I', "..");
        table.Add('J', ".---");
        table.Add('K', "-.-");
        table.Add('L', ".-..");
        table.Add('M', "--");
        table.Add('N', "-.");
        table.Add('O', "---");
        table.Add('P', ".--.");
        table.Add('Q', "--.-");
        table.Add('R', ".-.");
        table.Add('S', "...");
        table.Add('T', "-");
        table.Add('U', "..-");
        table.Add('V', "...-");
        table.Add('W', ".--");
        table.Add('X', "-..-");
        table.Add('Y', "-.--");
        table.Add('Z', "--..");

        table.Add('À', ".--.-");
        table.Add('Â', ".--.-");
        table.Add('Æ', ".-.-");
        table.Add('Ç', "-.-..");
        table.Add('È', ".-..-");
        table.Add('Ë', "..-..");
        table.Add('É', "..-..");
        table.Add('Ê', "-..-.");
        table.Add('Ï', "-..--");
        table.Add('Ô', "---.");
        table.Add('Ü', "..--");
        table.Add('Ù', "..--");

        // Numerics
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

        // Punctuation & Special Characters
        table.Add('.', ".-.-.-");
        table.Add(',', "--..--");
        table.Add('?', "..--..");
        table.Add(';', "-.-.-.");
        table.Add(':', "---...");
        table.Add('/', "-..-.");
        table.Add('\'', ".----.");
        table.Add('\"', ".-..-.");
        table.Add('&', ".-...");
        table.Add('$', "...-..-");
        table.Add('@', ".--.-.");
        table.Add('¿', "..-.-");
        table.Add('¡', "--...-");
        table.Add('!', "-.-.--");
        table.Add('_', "..--.-");
        table.Add('+', ".-.-.");
        table.Add('-', "-....-");
        table.Add('=', "-...-");
        table.Add(')', "-.--.-");
        table.Add('(', "-.--.");

        return table;
    }

    internal static MorseTableReverse256 FrenchReversedTable()
    {
        var table = new MorseTableReverse256();

        // Alphabets
        table.Add(".-", 'A');
        table.Add("-...", 'B');
        table.Add("-.-.", 'C');
        table.Add("-..", 'D');
        table.Add(".", 'E');
        table.Add("..-.", 'F');
        table.Add("--.", 'G');
        table.Add("....", 'H');
        table.Add("..", 'I');
        table.Add(".---", 'J');
        table.Add("-.-", 'K');
        table.Add(".-..", 'L');
        table.Add("--", 'M');
        table.Add("-.", 'N');
        table.Add("---", 'O');
        table.Add(".--.", 'P');
        table.Add("--.-", 'Q');
        table.Add(".-.", 'R');
        table.Add("...", 'S');
        table.Add("-", 'T');
        table.Add("..-", 'U');
        table.Add("...-", 'V');
        table.Add(".--", 'W');
        table.Add("-..-", 'X');
        table.Add("-.--", 'Y');
        table.Add("--..", 'Z');

        table.Add(".--.-", 'À');
        table.Add(".-.-", 'Æ');
        table.Add("-.-..", 'Ç');
        table.Add(".-..-", 'È');
        table.Add("..-..", 'Ë');
        table.Add("-..--", 'Ï');
        table.Add("---.", 'Ô');
        table.Add("..--", 'Ü');

        // Numerics
        table.Add(".----", '1');
        table.Add("..---", '2');
        table.Add("...--", '3');
        table.Add("....-", '4');
        table.Add(".....", '5');
        table.Add("-....", '6');
        table.Add("--...", '7');
        table.Add("---..", '8');
        table.Add("----.", '9');
        table.Add("-----", '0');

        // Space
        table.Add("/", ' ');

        // Punctuation & Special Characters
        table.Add(".-.-.-", '.');
        table.Add("--..--", ',');
        table.Add("..--..", '?');
        table.Add("-.-.-.", ';');
        table.Add("---...", ':');
        table.Add("-..-.", '/');
        table.Add(".----.", '\'');
        table.Add(".-..-.", '\"');
        table.Add(".-...", '&');
        table.Add("...-..-", '$');
        table.Add(".--.-.", '@');
        table.Add("..-.-", '¿');
        table.Add("--...-", '¡');
        table.Add("-.-.--", '!');
        table.Add("..--.-", '_');
        table.Add(".-.-.", '+');
        table.Add("-....-", '-');
        table.Add("-...-", '=');
        table.Add("-.--.-", ')');
        table.Add("-.--.", '(');

        return table;
    }
}
