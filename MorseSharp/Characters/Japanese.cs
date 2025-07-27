namespace MorseSharp.Characters;

internal static class JapaneseCharacters
{
    internal static MorseTable256 JapaneseTable()
    {
        var table = new MorseTable256();

        // Japanese Alphabets (Katakana)
        table.Add('ア', "--.--");
        table.Add('カ', ".-..");
        table.Add('サ', "-.-.-");
        table.Add('タ', "-.");
        table.Add('ナ', ".-.");
        table.Add('ハ', "-...");
        table.Add('マ', "-..-");
        table.Add('ヤ', ".--");
        table.Add('ラ', "...");
        table.Add('ワ', "-.-");
        table.Add('イ', ".-");
        table.Add('キ', "-.-..");
        table.Add('シ', "--.-.");
        table.Add('チ', "..-.");
        table.Add('ニ', "-.-.");
        table.Add('ヒ', "--..-");
        table.Add('ミ', "..-.-");
        table.Add('リ', "--.");
        table.Add('ヰ', ".-..-");
        table.Add('ウ', "..-");
        table.Add('ク', "...-");
        table.Add('ス', "---.-");
        table.Add('ツ', ".--.");
        table.Add('ヌ', "....");
        table.Add('フ', "--..");
        table.Add('ム', "-");
        table.Add('ユ', "-..--");
        table.Add('ル', "-.--.");
        table.Add('ン', ".-.-.");
        table.Add('エ', "-.---");
        table.Add('ケ', "-.--");
        table.Add('セ', ".---.");
        table.Add('テ', ".-.--");
        table.Add('ネ', "--.-");
        table.Add('ヘ', ".");
        table.Add('メ', "-...-");
        table.Add('レ', "---");
        table.Add('ヱ', ".--..");
        table.Add('オ', ".-...");
        table.Add('コ', "----");
        table.Add('ソ', "---.");
        table.Add('ト', "..-..");
        table.Add('ノ', "..--");
        table.Add('ホ', "-..");
        table.Add('モ', "-..-.");
        table.Add('ヨ', "--");
        table.Add('ロ', ".-.-");
        table.Add('ヲ', ".---");
        table.Add('゛', "..");
        table.Add('゜', "..--.");
        table.Add('。', ".-.-..");
        table.Add('ー', ".--.-");
        table.Add('、', ".-.-.-");

        // Punctuation
        table.Add('（', "-.--.-");
        table.Add('）', ".-..-.");

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

        return table;
    }

    internal static MorseTableReverse256 JapaneseReversedTable()
    {
        var table = new MorseTableReverse256();

        // Japanese Alphabets (Katakana)
        table.Add("--.--", 'ア');
        table.Add(".-..", 'カ');
        table.Add("-.-.-", 'サ');
        table.Add("-.", 'タ');
        table.Add(".-.", 'ナ');
        table.Add("-...", 'ハ');
        table.Add("-..-", 'マ');
        table.Add(".--", 'ヤ');
        table.Add("...", 'ラ');
        table.Add("-.-", 'ワ');
        table.Add(".-", 'イ');
        table.Add("-.-..", 'キ');
        table.Add("--.-.", 'シ');
        table.Add("..-.", 'チ');
        table.Add("-.-.", 'ニ');
        table.Add("--..-", 'ヒ');
        table.Add("..-.-", 'ミ');
        table.Add("--.", 'リ');
        table.Add(".-..-", 'ヰ');
        table.Add("..-", 'ウ');
        table.Add("...-", 'ク');
        table.Add("---.-", 'ス');
        table.Add(".--.", 'ツ');
        table.Add("....", 'ヌ');
        table.Add("--..", 'フ');
        table.Add("-", 'ム');
        table.Add("-..--", 'ユ');
        table.Add("-.--.", 'ル');
        table.Add(".-.-.", 'ン');
        table.Add("-.---", 'エ');
        table.Add("-.--", 'ケ');
        table.Add(".---.", 'セ');
        table.Add(".-.--", 'テ');
        table.Add("--.-", 'ネ');
        table.Add(".", 'ヘ');
        table.Add("-...-", 'メ');
        table.Add("---", 'レ');
        table.Add(".--..", 'ヱ');
        table.Add(".-...", 'オ');
        table.Add("----", 'コ');
        table.Add("---.", 'ソ');
        table.Add("..-..", 'ト');
        table.Add("..--", 'ノ');
        table.Add("-..", 'ホ');
        table.Add("-..-.", 'モ');
        table.Add("--", 'ヨ');
        table.Add(".-.-", 'ロ');
        table.Add(".---", 'ヲ');
        table.Add("..", '゛');
        table.Add("..--.", '゜');
        table.Add(".-.-..", '。');
        table.Add(".--.-", 'ー');
        table.Add(".-.-.-", '、');

        // Punctuation
        table.Add("-.--.-", '（');
        table.Add(".-..-.", '）');

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
        table.Add("--..--", ',');
        table.Add("..--..", '?');
        table.Add("-.-.-.", ';');
        table.Add("---...", ':');
        table.Add(".----.", '\'');
        table.Add("...-..-", '$');
        table.Add(".--.-.", '@');
        table.Add("--...-", '¡');
        table.Add("-.-.--", '!');

        // Special Characters
        table.Add("..--.-", '_');
        table.Add("-....-", '-');

        return table;
    }
}
