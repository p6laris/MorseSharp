using MorseSharp;
using MorseSharp.MorseConverter;
using System.IO;
using System.Windows;

MorseTextConverter converter = new MorseTextConverter();
MorseAudioConverter audioConverter = new MorseAudioConverter();

string message = "Hello Morse";
string morse = string.Empty;

try
{
    morse = await converter.ConvertToMorseEnglish(message);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.WriteLine(morse);