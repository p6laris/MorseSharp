# MorseSharp
[![NuGet](https://img.shields.io/nuget/dt/MorseSharp?logo=nuget)](https://www.nuget.org/packages/MorseSharp)
![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/p6laris/MorseSharp)

MorseSharp is a fast, allocation-free .NET library that encodes and decodes Morse code in **11 languages** including
Kurdish, and turns the dots and dashes into WAV audio or light-blink sequences.

![alt text](https://github.com/p6laris/MorseSharp/blob/master/MorseSharp.png?raw=true)

Requires **.NET 10**. It has no NuGet dependencies and is trimming and native-AOT friendly.
For .NET 8 and 9, use MorseSharp 5.x.

## Supported Languages

| Language      | Enum Value              |
|---------------|-------------------------|
| English       | `Language.English`      |
| Kurdish       | `Language.Kurdish`      |
| Kurdish Latin | `Language.KurdishLatin` |
| Arabic        | `Language.Arabic`       |
| Deutsch       | `Language.Deutsch`      |
| Espanol       | `Language.Spanish`      |
| Francais      | `Language.French`       |
| Italiano      | `Language.Italian`      |
| Japanese      | `Language.Japanese`     |
| Portugues     | `Language.Portugues`    |
| Russian       | `Language.Russian`      |

NOTE: All language sources are obtained from [MorseCoder](https://morsedecoder.com/), except for Kurdish, Kurdish Latin
[More info](https://github.com/p6laris/MorseSharp/blob/master/KurdishToMorse.md) and Russian obtained from this
[wiki](https://en.wikipedia.org/wiki/Russian_Morse_code).
If you encounter any issues with the obtained characters or have suggestions for improvement, please feel free to
[open an issue](https://github.com/p6laris/MorseSharp/issues) in this repository.

## Installation

```bash
Install-Package MorseSharp
```

## Usage

Start from the singleton returned by `GetConverter()` and pick a language with `ForLanguage`.

```C#
using MorseSharp;

var conv = Morse.GetConverter()
     .ForLanguage(Language.English);
```

The chain is thread-safe: state lives per thread, so many threads can convert at the same time. Finish a chain on the
thread that started it.

## Text

#### Encoding

`ToMorse` validates the text, `Encode` produces the string. Characters are separated by a space and words by `/`.

```C#
var morse = Morse.GetConverter()
    .ForLanguage(Language.English)
    .ToMorse("Hi")
    .Encode();     // ".... .."
```

:warning: **CharacterNotPresentedException** is thrown when a character has no Morse representation in that language.

#### Decoding

> :exclamation: ``Words are separated by ( / ), letters by whitespace.``

```C#
var text = Morse.GetConverter()
    .ForLanguage(Language.English)
    .Decode(".... ..");   // "HI"
```

:warning: **SequenceNotFoundException** is thrown when a sequence has no character in that language.

## Audio

Audio is 16-bit PCM mono WAV at 11.025 kHz. Timing follows the ARRL Farnsworth standard: set `wordSpeed` below
`charSpeed` to keep crisp characters while stretching the gaps for practice.

Four ways to get the bytes, from most to least convenient:

```C#
var audio = Morse.GetConverter()
    .ForLanguage(Language.English)
    .ToMorse("Hello Morse")
    .ToAudio()
    .SetAudioOptions(charSpeed: 25, wordSpeed: 15, frequency: 700);

byte[] wav = audio.GetBytes();          // one exactly sized allocation

int size = audio.GetByteCount();        // size the buffer yourself
byte[] buffer = new byte[size];
int written = audio.GetBytes(buffer);   // zero allocation

using var file = File.Create("hi.wav");
audio.WriteTo(file);                    // zero allocation, pooled staging buffer
```

If you already have the Morse string, skip the encoding step:

```C#
Morse.GetConverter()
    .ForLanguage(Language.English)
    .ToAudio(".... ..")
    .SetAudioOptions(25, 25, 600)
    .GetBytes();
```

:warning: The character speed must be greater than or equal to the word speed, otherwise a
**SmallerCharSpeedException** is thrown. Speeds must be positive and the frequency must be below 5512.5 Hz.

## Light

`DoBlinks` calls your action with `true` when the light goes on and `false` when it goes off, in real time.
Timing is drift-compensated, and the callback runs on the captured synchronization context so you can touch UI directly.
The final callback is always `false`.

```C#
using var cts = new CancellationTokenSource();

await Morse.GetConverter()
    .ForLanguage(Language.Kurdish)
    .ToMorse("سڵاو")
    .ToLight()
    .SetBlinkerOptions(25, 25)
    .DoBlinks(on => { /* switch the light */ }, cts.Token);

// Or pass the Morse directly.
await Morse.GetConverter()
    .ForLanguage(Language.English)
    .ToLight(".... ..")
    .SetBlinkerOptions(25, 25)
    .DoBlinks(on => Console.BackgroundColor = on ? ConsoleColor.White : ConsoleColor.Black);
```

Cancelling switches the light off before the task is cancelled.

## Custom alphabets

If a language is missing, or you want one of the built-in ones with a tweak, build your own rather than forking:

```C#
var klingon = new MorseAlphabetBuilder("Klingon")
    .Add('a', ".-")
    .Add('b', "-...")
    .Build();

string morse = Morse.GetConverter()
    .ForAlphabet(klingon)
    .ToMorse("ab")
    .Encode();
```

`From` starts with an existing language so you can extend or trim it:

```C#
var extended = MorseAlphabetBuilder.From(Language.Deutsch)
    .Add('Ə', "..--.")
    .Remove('$')
    .Build();
```

- `Add` maps a character to a pattern and claims that pattern for decoding.
- `AddAlias` maps a character that shares an existing pattern. It encodes, but the pattern keeps decoding to the
  character added with `Add`, which is how `ß` and `ẞ` both work in German.
- `Remove` drops every entry for a character. If an alias was sharing the removed character's pattern, it takes
  the pattern over.

Patterns may be up to 8 symbols of `.` and `-`. Both letter cases are accepted when encoding, and decoding returns
the character exactly as you registered it. Alphabets are immutable once built and safe to share between threads,
so hold one in a static field and reuse it.

## Example

```C#
using MorseSharp;

try
{
    var morse = Morse.GetConverter()
        .ForLanguage(Language.English)
        .ToMorse("Hi")
        .Encode();

    var text = Morse.GetConverter()
        .ForLanguage(Language.English)
        .Decode(".... ..");

    using (var file = File.Create("hi.wav"))
    {
        Morse.GetConverter()
            .ForLanguage(Language.English)
            .ToMorse("Hi")
            .ToAudio()
            .SetAudioOptions(25, 25, 700)
            .WriteTo(file);
    }

    await Morse.GetConverter()
        .ForLanguage(Language.English)
        .ToLight(".... ..")
        .SetBlinkerOptions(25, 25)
        .DoBlinks(on => Console.BackgroundColor = on ? ConsoleColor.White : ConsoleColor.Black);
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
```

## Upgrading from 5.x

See [CHANGELOG.md](CHANGELOG.md). The short version: `GetBytes(out Span<byte>)` handed out a span over a pooled array
that had already been returned to the pool, so it is obsolete; use `GetBytes()`, `GetBytes(Span<byte>)` or `WriteTo`.
Timing was also wrong in 5.x, so generated audio is shorter for the same speeds.

## License
[MIT License](LICENSE)
