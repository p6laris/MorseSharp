# MorseSharp
MorseSharp is a simple lightweight .NET library to convert **english** and **kurdish** sentences to morse code.

## Installation
Use nuget package manager to install [MorseSharp](https://www.nuget.org/packages/MorseSharp) using CLI.
```bash
dotnet add package MorseSharp --version 1.0.1
```
## Usage
### 1. Text
1. You can use MorseSharp just by instantiating `MorseTextConverter` class

```C#
using MorseSharp;
MorseTextConverter converter = new MorseTextConverter();
```
2.__English__ sentences can be converted to morse code by just calling asynchronous method `ConvertToMorseEnglish`

```C#
using MorseSharp;
var morse = await converter.ConvertToMorse;
```
3. You can also convert __kurdish__ sentences to morse code by calling asynchronous method `ConvertToMorseKurdish`

## Example 
### Text to morse
This piece of code convert a simple text to morse code and then show it to the console:
```C#
using System;
using MorseSharp;

MorseTextConverter converter = new();
string morseEng = string.Empty;
string morseKrd = string.Empty;
try
{
   morseEng = await converter.ConvertToMorseEnglish("Hello Morse");
   morseKrd = await converter.ConvertToMorseEnglish(سڵاو مۆرس);
}
catch(Exception ex)
{
   Console.WriteLine(ex.Message);
}
Console.WriteLine(morseEng);
Console.WriteLine("--------------------------");
Console.WriteLine(morseKrd);
```
### Generating audio
1. To generate audio first you need to instantiate ``MorseAudioConverter``, there are five overloaded constructor
To configure audio options like language and characters speed, word speed ,frequency. this WinForm example demonstrate the purpose:
> MorseAudioConverter is just optimized wrapper of [jstoddard](https://github.com/jstoddard)'s [CWLibrary](https://github.com/jstoddard/CWLibrary) Library.
```C#
using MorseSharp;

MorseAudioConverter converter = new(Language.English,25,20,600);
//Or for kurdish language
//MorseAudioConverter converter = new(Language.Kurdish,25,20,600);

```
2. Bytes for the generated wav audio can be recived by calling ``MorseAudioConverter``'s asynchronous ``ConvertMorseToAudio`` method:
```C#
try
{
   var morse = await converter.ConvertMorseToAudio("Hello Morse");
}
catch(Exception ex)
{
   MessageBox.Show(ex.Message);
}
```
3.After getting the bytes you can stream the bytes and play the sound, this example use's SoundPlayer object to play the sound:
```C#
SoundPlayer player = new();
using(Stream stream = new MemoryStream(morse.ToArray())
{
   player.Stream = stream;
   player.PlayAsync();
}
```
## License
[MIT License](LICENSE)
