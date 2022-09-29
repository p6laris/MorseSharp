# MorseSharp
MorseSharp is a simple lightweight .NET library to convert **english** and **kurdish** sentences to morse code.

## Installation
Use nuget package manager to install [MorseSharp](https://www.nuget.org/packages/MorseSharp) using CLI.
```bash
dotnet add package MorseSharp --version 1.0.1
```
## Usage
1.You can use MorseSharp easily just by instantiating `MorseTextConverter` class

```C#
using MorseSharp;
MorseTextConverter converter = new MorseTextConverter();
```
2.Sentences can be converte to morse code by just calling asynchronous method `ConvertToMorse`

```C#
using MorseSharp;
var morse = await converter.ConvertToMorse;
```
## Example 
### Text to morse
This piece of code convert a simple text to morse code and then show it to the console:
```C#

using System;
using MorseSharp;

MorseTextConverter converter = new();
try
{
   var morse = await converter.ConvertToMorse();
   Console.WriteLine(morse);
}
catch(Exception ex)
{
   Console.WriteLine(ex.Message);
}
Console.WriteLine(morse);
```
### Generating audio
1.To generate audio first you need to instantiate ``MorseAudioConverter``, there are four overloaded constructor
To configure audio options like characters speed, word speed and frequency. this WinForm example demonstrate the purpose:
```C#
using MorseSharp;

MorseAudioConverter converter = new(25,20,600);
```
2.Bytes for the generated wav audio can be recived by calling ``MorseAudioConverter``'s asynchronous ``ConvertMorseToAudio`` method:
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
3.After getting the bytes you can stream the bytes and play the sound, in this example we are using SoundPlayer object to play the sound:
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
