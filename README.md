# MorseSharp
![Nuget](https://img.shields.io/nuget/dt/MorseSharp?logo=nuget)
![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/p6laris/MorseSharp)

MorseSharp is a .NET library to encoding/decoding  **up to 8 languages** including kurdish and generating audio morse.

![alt text](https://github.com/p6laris/MorseSharp/blob/master/MorseSharp.png?raw=true)

## Installation
Use nuget package manager to install [MorseSharp](https://www.nuget.org/packages/MorseSharp).
```bash
Install-Package MorseSharp
```
## Usage
### 1. Text
You can decode/encode use MorseSharp just by instantiating `TextMorseConverter` class and pass 'Language' enum to specify the language.

```C#
using MorseSharp;
TextMorseConverter converter = new TextMorseConverter(Language.English);
```

#### Encoding
Calling ConvertTextToMorse method and pass the text to encode the morse:

```C#
using MorseSharp;
var morse = await converter.ConvertTextToMorse("Hello");
```


#### Decoding
Calling asynchronous methods `ConvertMorseToText` to decode the morse:
 > :exclamation: Words must be separated by spaces, words by ( / ), Letters by space " ".

```C#
using MorseSharp;

var sentence = await converter.ConvertMorseToText(".... ...");
```

### 2. Generating audio
1. To generate audio first you need to instantiate ``MorseAudioConverter``, there are five overloaded constructor
To configure audio options such as language and characters speed, word speed ,frequency. this WinForm example demonstrate the purpose:
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
using(Stream stream = new MemoryStream(morse))
{
   player.Stream = stream;
   player.PlayAsync();
}
```
## Example 
This piece of code encode and decode's the morse and then show it to the console:
```C#
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
```

## License
[MIT License](LICENSE)
