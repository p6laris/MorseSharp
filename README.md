# MorseSharp
[![CI](https://github.com/p6laris/MorseSharp/actions/workflows/ci.yml/badge.svg)](https://github.com/p6laris/MorseSharp/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/dt/MorseSharp?logo=nuget)](https://www.nuget.org/packages/MorseSharp)
![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/p6laris/MorseSharp)

MorseSharp is a fast, allocation-free .NET library for Morse code in **11 languages** including Kurdish. It encodes
and decodes text, generates or decodes 16-bit PCM WAV audio, drives or reads a blinking light, keys input from an
iambic paddle, lets you define your own alphabet, and generates Koch-method lessons and practice traffic.

![MorseSharp logo](https://raw.githubusercontent.com/p6laris/MorseSharp/master/MorseSharp.png)

Requires **.NET 10**. It has no NuGet dependencies and is trimming and native-AOT friendly.
For .NET 8 and 9, use MorseSharp 5.x.

## Contents

- [Supported Languages](#supported-languages)
- [Installation](#installation)
- [Usage](#usage)
- [Text](#text)
  - [Encoding](#encoding)
  - [Decoding](#decoding)
- [Audio](#audio)
  - [Output format](#output-format)
- [Light](#light)
  - [Elements](#elements)
- [Keying](#keying)
- [Practice](#practice)
- [Adding a language](#adding-a-language)
- [Decoding received audio](#decoding-received-audio)
  - [Live audio](#live-audio)
- [Prosigns](#prosigns)
- [Custom alphabets](#custom-alphabets)
- [Example](#example)
- [Upgrading from 5.x](#upgrading-from-5x)
- [License](#license)

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
**SmallerCharSpeedException** is thrown. Speeds must be positive and the frequency must be below half the sample rate.

#### Output format

Pass an `AudioFormat` to change the sample rate, channels, bit depth, or how long each element fades:

```C#
.SetAudioOptions(25, 25, 700, new AudioFormat(
    SampleRate: 44100,
    Channels: 2,
    BitDepth: AudioBitDepth.Float32,
    EdgeMilliseconds: 5))
```

Defaults are 11.025 kHz mono 16-bit, matching what earlier versions produced.

`EdgeMilliseconds` is the one that changes what you hear. Switching a tone on and off instantly makes the waveform
jump from full amplitude to nothing between one sample and the next, and that step is audible as a click at both ends
of every dot and dash. Fading over a few milliseconds removes it. It defaults to 5 ms; set it to 0 for the old
behaviour.

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

### Elements

`DoBlinks` drives the loop for you, which is all an LED needs. When you want the loop yourself, take the elements.

```C#
await foreach (var element in Morse.GetConverter()
    .ForLanguage(Language.English)
    .ToMorse("SOS")
    .ToLight()
    .SetBlinkerOptions(25, 25)
    .PlayAsync(cts.Token))
{
    await relay.SetAsync(element.KeyDown);   // await inside the loop, unlike a callback
}
```

Each `MorseElement` carries its `Kind` (`Dot`, `Dash`, `ElementGap`, `CharGap`, `WordGap`), its `Duration`, and
`KeyDown` for the on/off state. `PlayAsync` paces them in real time with the same drift compensation as `DoBlinks`;
`GetElements()` hands over the whole sequence at once for anything that does its own timing, such as drawing a
timeline or a game loop, and enumerating it allocates nothing.

```C#
foreach (var element in Morse.GetConverter()
    .ForLanguage(Language.English).ToLight("... --- ...").SetBlinkerOptions(25, 25).GetElements())
{
    Console.WriteLine($"{element.Kind} for {element.Duration.TotalMilliseconds} ms");
}
```

A sequence is a snapshot. It outlives the chain that produced it, and you can walk it as many times as you like.

## Keying

`IambicKeyer` goes the other way: paddle presses in, timed elements out.

```C#
var keyer = new IambicKeyer(wordsPerMinute: 25, KeyerMode.B);
var decoder = new KeyerDecoder(Language.English, 25);

// wherever the hardware is polled, on whatever thread
keyer.Paddles(dot: dotContactClosed, dash: dashContactClosed);

// wherever elements are played
while (keyer.TryRead(out var element))
{
    sidetone.Set(element.KeyDown);
    await Task.Delay(element.Duration);
    decoder.Add(element);
}

decoder.Quiet(silenceSoFar);
while (decoder.TryRead(out char character, out string? prosign))
    Console.Write(prosign ?? character.ToString());
```

Holding one contact repeats that element. Squeezing both alternates between them; that's the iambic part. A tap on
the opposite contact during an element is remembered rather than dropped, so a short tap isn't swallowed.

`KeyerMode` matters more than the other settings here. Let go of a squeeze and mode A stops after the element in
progress, while mode B sends one more of the opposite kind. Most operators build muscle memory around one or the
other and stick with it.

There is no clock inside the keyer. It says what to send and for how long; you decide when. That keeps it usable from
a game loop, a GPIO interrupt or a test, and `Paddles` is safe to call from a different thread than `TryRead`.

## Practice

`Koch` generates lessons for the method of the same name: full target speed from the first lesson, starting with two
characters and adding one each time you copy better than ninety per cent.

```C#
string lesson = Koch.Generate(level: 5, groups: 10);   // "KMRSU MKSRU RUKSM ..."

byte[] wav = Morse.GetConverter().ForLanguage(Language.English)
    .ToMorse(lesson).ToAudio().SetAudioOptions(20, 20).GetBytes();

var score = Koch.Score(lesson, whatTheLearnerTyped);
if (score.ClearsThreshold)
    level++;
```

Scoring ignores spacing and case. It's positional though: drop a character and everything after it shifts out of
line, which counts against you. `Koch.Order` is the sequence characters are introduced in; `Koch.MaxLevel` is how
many there are.

`Callsign` and `Qso` produce practice material shaped like real traffic rather than random letters.

```C#
string call = Callsign.Next();       // "DL4KRM"

foreach (string transmission in Qso.Generate())
    Console.WriteLine(transmission);

// CQ CQ CQ DE YO3WF YO3WF YO3WF K
// YO3WF DE 9A6TN 9A6TN K
// 9A6TN DE YO3WF = GM = TNX FER CALL = UR RST 599 = NAME KOZHEN = QTH HAWLER = HW? = 9A6TN DE YO3WF K
// ...
// 9A6TN DE YO3WF = TNX FER QSO = 73 ES CUL = 9A6TN DE YO3WF <SK>
```

Every transmission is ready to pass straight to `ToMorse`, `=` and the closing `<SK>` included; both are keyed as
single unbroken signals. Each generator takes an optional `Random` too. Seed one and you'll get the same lesson,
callsign or contact back. `Koch.Generate` and `Callsign.Next` also have `Span<char>` overloads if you don't want the
allocation.

## Adding a language

The built-in alphabets live in `MorseSharp/Alphabet/Data/*.morse`, one file per language, as plain
`character<tab>pattern` lines under `[PRIMARY]` and `[ALIAS]` sections. A source generator packs them into lookup
tables at build time. Adding or correcting a character means editing a data file, not writing code, and a mistake
like two characters sharing a pattern shows up as a build error on the offending line.

## Decoding received audio

The library can listen as well as send. Give it 16-bit PCM mono samples and the tone you expect, and it decodes them
back to text:

```C#
string text = Morse.GetConverter()
    .ForLanguage(Language.English)
    .FromAudio(samples, sampleRate: 11025, frequency: 700, wordsPerMinute: 20);
```

`wordsPerMinute` only has to be roughly right; the decoder measures the real speed from the signal and keeps
adjusting as it goes. That's what lets it follow hand-sent Morse that drifts, and handle Farnsworth spacing where the
characters are fast but the gaps are stretched. A sequence with no character in the selected alphabet decodes to `?`.

If the frequency you ask for carries no real tone, you get an empty string rather than invented characters.

### Live audio

For audio arriving a piece at a time, from a microphone or a receiver, use the streaming decoder. Buffer sizes need
not line up with anything:

```C#
var decoder = Morse.GetConverter()
    .ForLanguage(Language.English)
    .CreateAudioDecoder(sampleRate: 11025, frequency: 700, wordsPerMinute: 20);

while (recording)
{
    int read = microphone.Read(buffer);
    decoder.Write(buffer.AsSpan(0, read));

    while (decoder.TryRead(out char character))
        Console.Write(character);
}

decoder.Flush();   // the last character has no gap after it to announce it finished
```

A streaming decoder can't see ahead. Instead it re-measures the tone threshold and the sender's timing from a
sliding window of what it heard recently, which means the stated speed matters a little more here than it does for a
whole recording, since it's what seeds the first few characters before there's enough signal to measure the real one.

## Prosigns

Prosigns are procedural signals: two or more letters keyed as one unbroken sequence, with no gap between them. They
are instructions rather than text. Write them in angle brackets:

```C#
var morse = Morse.GetConverter()
    .ForLanguage(Language.English)
    .ToMorse("CQ CQ <AR>")
    .Encode();
```

`<AR>` keys as a single signal; it isn't the same as sending `A` then `R`.

English defines these:

| Prosign | Meaning | Pattern | |
|---|---|---|---|
| `<SK>` | end of contact | `...-.-` | decodes back as `<SK>` |
| `<SN>` | understood | `...-.` | decodes back as `<SN>` |
| `<CT>` | attention, starting | `-.-.-` | decodes back as `<CT>` |
| `<HH>` | correction | `........` | decodes back as `<HH>` |
| `<AR>` | end of message | `.-.-.` | same signal as `+`, which keeps the pattern |
| `<BT>` | break | `-...-` | same signal as `=` |
| `<KN>` | go ahead, named station | `-.--.` | same signal as `(` |
| `<AS>` | wait | `.-...` | same signal as `&` |

The last four genuinely are the same on-air signal as the punctuation beside them. Either spelling encodes, but the
punctuation keeps ownership of the pattern for decoding. Add your own with `MorseAlphabetBuilder.AddProsign`, or
`AddProsignAlias` when the pattern is already taken.

:warning: Patterns are capped at 8 symbols, so `SOS` doesn't fit as a single 9-symbol prosign. Sent as the three
separate letters `S`, `O`, `S` (what nearly everyone means anyway) it's unaffected.

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

A quick look at the basics:

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

Custom alphabets, prosigns, decoding audio (buffered and streaming), the element stream, the iambic keyer, and the
Koch/callsign/QSO practice generators aren't shown here. For those, see [`Example/`](Example): a runnable console
demo with one file per feature under `Example/Demos/`. [`AudioExample/`](AudioExample) covers the same ground behind
a small WinForms UI.

## Upgrading from 5.x

See [CHANGELOG.md](CHANGELOG.md). The short version: `GetBytes(out Span<byte>)` handed out a span over a pooled array
that had already been returned to the pool, so it is obsolete; use `GetBytes()`, `GetBytes(Span<byte>)` or `WriteTo`.
Timing was also wrong in 5.x, so generated audio is shorter for the same speeds.

## License
[MIT License](LICENSE)
