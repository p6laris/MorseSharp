


TextMorseConverter Converter = new TextMorseConverter(Language.English);


string morse = string.Empty;
string text = string.Empty;

try
{
    morse =  Converter.ConvertTextToMorse("The quick brown fox jumps over the lazy dog");
    text = Converter.ConvertMorseToText("_ .... . / __._ .._ .. _._. _._ / _... ._. ___ .__ _. / .._. ___ _.._ / .___ .._ __ .__. ... / ___ ..._ . ._. / _ .... . / ._.. ._ __.. _.__ / _.. ___ __.");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.WriteLine(morse);
Console.WriteLine(text);