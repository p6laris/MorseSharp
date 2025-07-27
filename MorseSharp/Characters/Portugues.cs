namespace MorseSharp.Characters;

internal static class PortuguesCharacters
{
    internal static MorseTable256 PortuguesTable()
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

        table.Add('Ã', ".--.-");
        table.Add('Á', ".--.-");
        table.Add('À', ".--.-");
        table.Add('Â', ".--.-");
        table.Add('Ç', "-.-..");
        table.Add('É', "..-..");
        table.Add('Ê', "-..-.");
        table.Add('Ó', "---.");
        table.Add('Ô', "---.");
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

        // Punctuation
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

        // Special Characters
        table.Add('_', "..--.-");
        table.Add('+', ".-.-.");
        table.Add('-', "-....-");
        table.Add('=', "-...-");
        table.Add(')', "-.--.-");
        table.Add('(', "-.--.");

        return table;
    }

    internal static MorseTableReverse256 PortuguesReversedTable()
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

        table.Add(".--.-", 'Ã');
        table.Add("-.-..", 'Ç');
        table.Add("..-..", 'É');
        table.Add("---.", 'Ó');
        table.Add("..--", 'Ù');

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

        // Punctuation
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

        // Special Characters
        table.Add("..--.-", '_');
        table.Add(".-.-.", '+');
        table.Add("-....-", '-');
        table.Add("-...-", '=');
        table.Add("-.--.-", ')');
        table.Add("-.--.", '(');

        return table;
    }
}
