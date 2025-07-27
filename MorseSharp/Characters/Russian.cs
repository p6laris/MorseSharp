namespace MorseSharp.Characters;

internal static class RussianCharacters
{
    internal static MorseTable256 RussianTable()
    {
        var table = new MorseTable256();

        // Russian Cyrillic
        table.Add('А', ".-");
        table.Add('Б', "-...");
        table.Add('В', ".--");
        table.Add('Г', "--.");
        table.Add('Д', "-..");
        table.Add('Е', ".");
        table.Add('Ё', ".");
        table.Add('Ж', "...-");
        table.Add('З', "--..");
        table.Add('И', "..");
        table.Add('Й', ".---");
        table.Add('К', "-.-");
        table.Add('Л', ".-..");
        table.Add('М', "--");
        table.Add('Н', "-.");
        table.Add('О', "---");
        table.Add('П', ".--.");
        table.Add('Р', ".-.");
        table.Add('С', "...");
        table.Add('Т', "-");
        table.Add('У', "..-");
        table.Add('Ф', "..-.");
        table.Add('Х', "....");
        table.Add('Ц', "-.-.");
        table.Add('Ч', "---.");
        table.Add('Ш', "----");
        table.Add('Щ', "--.-");
        table.Add('Ъ', "-..-"); // same as Ь
        table.Add('Ы', "-.--");
        table.Add('Ь', "-..-");
        table.Add('Э', "..-..");
        table.Add('Ю', "..--");
        table.Add('Я', ".-.-");

        // Other Cyrillic (non-Russian)
        table.Add('Ї', ".---.");
        table.Add('Є', "..-..");
        table.Add('І', "..");
        table.Add('Ґ', "--.");

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

    internal static MorseTableReverse256 RussianReversedTable()
    {
        var table = new MorseTableReverse256();

        // Russian Cyrillic
        table.Add(".-", 'А');
        table.Add("-...", 'Б');
        table.Add(".--", 'В');
        table.Add("--.", 'Г');
        table.Add("-..", 'Д');
        table.Add(".", 'Е');
        table.Add("...-", 'Ж');
        table.Add("--..", 'З');
        table.Add("..", 'И');
        table.Add(".---", 'Й');
        table.Add("-.-", 'К');
        table.Add(".-..", 'Л');
        table.Add("--", 'М');
        table.Add("-.", 'Н');
        table.Add("---", 'О');
        table.Add(".--.", 'П');
        table.Add(".-.", 'Р');
        table.Add("...", 'С');
        table.Add("-", 'Т');
        table.Add("..-", 'У');
        table.Add("..-.", 'Ф');
        table.Add("....", 'Х');
        table.Add("-.-.", 'Ц');
        table.Add("---.", 'Ч');
        table.Add("----", 'Ш');
        table.Add("--.-", 'Щ');
        table.Add("-..-", 'Ъ'); // shared with Ь but included here only once
        table.Add("-.--", 'Ы');
        table.Add("..-..", 'Э');
        table.Add("..--", 'Ю');
        table.Add(".-.-", 'Я');

        // Other Cyrillic (non-Russian)
        table.Add(".---.", 'Ї');

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
