using MorseSharp;
using MorseSharp.Audio.Languages;
using MorseSharp.Converter;
using System.IO;
using System.Windows;



TextMorseConverter converter = new TextMorseConverter();
MorseTextConverter converter1 = new MorseTextConverter();

string englishMessage = "Hello Morse";
string kurdishMessage = "زانا";
string morseMessageEnglish = ".... ..";
string morseMessageKurdish = "_.... .__ ..";

string morseEnglish = string.Empty;
string morseKurdish = string.Empty;
string englishMorse = string.Empty;
string kurdishMorse =string.Empty;

try
{
   
    //Convert english to morse
    morseEnglish = await converter.ConvertToMorseEnglish(englishMessage);
    //Convert kurdish to morse
    morseKurdish = await converter.ConvertToMorseKurdish(kurdishMessage);
    //Convert morse to english sentence
    englishMorse = await converter1.ConvertMorseToEnglish(morseMessageEnglish);
    //Converts morse to kurdish sentence
    kurdishMorse = await converter1.ConvertMorseToKurdish(morseMessageKurdish);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

Console.WriteLine(morseEnglish);
Console.WriteLine(morseKurdish);
Console.WriteLine(englishMorse);
Console.WriteLine(kurdishMorse);