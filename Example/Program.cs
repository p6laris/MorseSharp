using MorseSharp;
using MorseSharp.MorseConverter;

MorseTextConverter converter = new MorseTextConverter();

string morse = string.Empty;
try
{
    morse = await converter.ConvertToMorse("Hello Morse");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.WriteLine(morse);