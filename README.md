# MorseSharp
![Nuget](https://img.shields.io/nuget/dt/MorseSharp?logo=nuget)
![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/p6laris/MorseSharp)

MorseSharp is a simple lightweight .NET library to Encoding/decoding  **english** and **kurdish** sentences to morse code and vice versa.

![alt text](https://github.com/p6laris/MorseSharp/blob/master/MorseSharp.png?raw=true)

## Installation
Use nuget package manager to install [MorseSharp](https://www.nuget.org/packages/MorseSharp).
```bash
Install-Package MorseSharp
```
## Usage
### 1. Text
1. You can use MorseSharp just by instantiating `TextMorseConverter` class

```C#
using MorseSharp;
TextMorseConverter converter = new TextMorseConverter();
```
2.__English__ sentences can be converted to morse code by just calling asynchronous method `ConvertToMorseEnglish`

```C#
using MorseSharp;
var morse = await converter.ConvertToMorseEnglish("Hi Morse");
```
3. You can also convert __kurdish__ sentences to morse code by calling asynchronous method `ConvertToMorseKurdish`

#### Decoding
You can decode morse messages by instantiating `MorseTextConverter` class, then calling asynchronous methods `ConvertKurdishToMorse` or `ConvertMorseToEnglish`:

```C#
using MorseSharp;
var converter = new MorseTextConverter();

var sentence = await converter.ConvertEnglishToMorse(".... ...");
```

### 2. Generating audio
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
## Example 
### Text to morse
This piece of code convert a simple text to morse code and then show it to the console:
```C#
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
```

## License
[MIT License](LICENSE)
