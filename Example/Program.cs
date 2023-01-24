using MorseSharp;
using MorseSharp.Converter;


TextMorseConverter Converter = new TextMorseConverter(Language.English);


string morse = string.Empty;
string text = string.Empty;

try
{
    morse = await Converter.ConvertTextToMorse("Hello World");
    text = await Converter.ConvertMorseToText(".... ..");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.WriteLine(morse);
Console.WriteLine(text);