using MorseSharp;
using MorseSharp.MorseConverter;
using System.IO;
using System.Windows;

//Create an instance of type MorseTextConverter
MorseTextConverter converter = new MorseTextConverter();

string englishMessage = "Hello Morse";
string kurdishMessage = "تاهر";
string morseEnglish = string.Empty;
string morseKurdish = string.Empty;

try
{
    //Convert english to morse
    morseEnglish = await converter.ConvertToMorseEnglish(englishMessage);
    //Convert kurdish to morse
    morseKurdish = await converter.ConvertToMorseKurdish(kurdishMessage);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.WriteLine(morseEnglish);
Console.WriteLine(morseKurdish);