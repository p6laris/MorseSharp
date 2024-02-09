# MorseSharp
![Nuget](https://img.shields.io/nuget/dt/MorseSharp?logo=nuget)
![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/p6laris/MorseSharp)

MorseSharp is a fast .NET library to encoding/decoding  **up to 10 languages** including kurdish and generating audio , blinking lights for morse dash and dots.

![alt text](https://github.com/p6laris/MorseSharp/blob/master/MorseSharp.png?raw=true)

## Supported Languages

| Language      | Enum Value   |
|---------------|--------------|
| English       | `Language.English` |
| Kurdish       | `Language.Kurdish` |
| Kurdish Latin | `Language.KurdishLatin` |
| Arabic        | `Language.Arabic` |
| Deutsch       | `Language.Deutsch` |
| Espanol       | `Language.Espanol` |
| Francais      | `Language.Francais` |
| Italiano      | `Language.Italiano` |
| Japanese      | `Language.Japanese` |
| Portugues     | `Language.Portugues` |
| Russian       | `Language.Russian` |

## Installation
Use nuget package manager to install [MorseSharp](https://www.nuget.org/packages/MorseSharp).
```bash
Install-Package MorseSharp
```
## Usage
Effortlessly decode/encode Morse code, generate audio, and control blinking lights using the fluent `Morse` class. Begin by obtaining a singleton instance through the `GetConverter()` method and specifying your desired language using the `ForLanguage` method and pass the `Language` enum to it.

```C#
using MorseSharp;

var conv = Morse.GetConverter()
     .ForLanguage(Language.English);
```

## Text
Once you've set the language via the `ForLanguage` method, you can decode/encode Morse code with a series of method calls.

#### Encoding
Utilize the `ToMorse` method to encode your text into Morse code, and then call the `Encode` method to obtain the Morse code as a string:

```C#
using MorseSharp;

 var morse = Morse.GetConverter()
     .ForLanguage(Language.English)
     .ToMorse("Hi")
     .Encode();
```

:warning: __WordNotPresentedException__ will be throw when a character in the input text does not have a corresponding Morse code representation.


#### Decoding
to decode Morse code using the `Decode` method:
 > :exclamation: ``Words must be separated by ( / ), Letters by space " ".``

```C#
using MorseSharp;

var text = Morse.GetConverter()
    .ForLanguage(Language.English)
    .Decode(".... ..");

```
:warning: __SequenceNotFoundException__ when an invalid Morse code sequence is encountered, and the corresponding character cannot be found.

## Audio
You have two options to generate audio:

#### By Encoding The Text
Encode your text using `ToMorse`, and then proceed through the chain to generate audio for the encoded text. After encoding the text, use the `ToAudio` method, set the audio options with `SetAudioOptions`, and finally, retrieve audio bytes using `GetBytes`:

```C#
using MorseSharp;

 Morse.GetConverter()
     .ForLanguage(Language.English)
     .ToMorse("Hello Morse")
     .ToAudio()
     .SetAudioOptions(25, 25, 600)
     .GetBytes(out Span<byte> morse);
```

#### Manually
If you already have the encoded text as a string, skip the encoding step and pass the encoded text directly to the overloaded `ToAudio` method:
```C#
using MorseSharp;

Morse.GetConverter()
    .ForLanguage(Language.English)
    .ToAudio(".... ..")
    .SetAudioOptions (25, 25, 600)
    .GetBytes(out Span<byte> morse);

```
:warning: The character speed must be greater than or equal to the word speed; otherwise, a __SmallerCharSpeedException__ will be thrown.

## Light
The class can also be able to blink lights to a specific morse. Just like the audio you have to options to blink lights either by Encoding it first or by set the dash and dots directly to the method and skip the encoding part:

```C#
using MorseSharp;

 //By Encoding it then blink the lights.
Morse.GetConverter()
    .ForLanguage(Language.English)
    .ToMorse(null)
    .ToLight()
    .SetBlinkerOptions(25, 25)
    .DoBlinks((hasToBlink) => {
       //Do something
    });

//By directly pass the morse to method.
 await Morse.GetConverter()
     .ForLanguage(Language.English)
     .ToLight(".... ..")
     .SetBlinkerOptions(25, 25)
     .DoBlinks((hasToBlink) =>
     {
         if (hasToBlink)
             Console.BackgroundColor = ConsoleColor.White;
         else 
             Console.BackgroundColor = ConsoleColor.Black;
     });
```
You need to set the character speed and word speed using `SetBlinkerOptions`, then invoke async `DoBlinks` and subscribe to the `Action<bool> parameter`.

## Example 
This piece of code encode and decode's the morse and then show it to the console:
```C#
using MorseSharp;


try
{
    //Encoding
    var morse = Morse.GetConverter()
        .ForLanguage(Language.English)
        .ToMorse("Hi")
        .Encode();

    //Decoding
    var text = Morse.GetConverter()
        .ForLanguage(Language.English)
        .Decode(".... ..");

    //Light Blinking
    await Morse.GetConverter()
        .ForLanguage(Language.English)
        .ToLight(".... ..")
        .SetBlinkerOptions(25, 25)
        .DoBlinks((hasToBlink) =>
        {
            if (hasToBlink)
                Console.BackgroundColor = ConsoleColor.White;
            else 
                Console.BackgroundColor = ConsoleColor.Black;
        });

}
catch(Exception ex)
{
    Console.WriteLine(ex.Message);
}
```

## License
[MIT License](LICENSE)
