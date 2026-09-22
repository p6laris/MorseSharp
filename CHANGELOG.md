# Changelog

## 6.1.0 (2026-09-22)

### Added

- **Inspecting a built-in alphabet.** `MorseAlphabet.ForLanguage(Language)` returns the built-in alphabet for a
  language directly, and `MorseAlphabet.Characters` lists every character it maps alongside the pattern each is
  keyed as and whether it is alias (encode-only, sharing a pattern another character owns for decoding):

  ```csharp
  MorseAlphabet english = MorseAlphabet.ForLanguage(Language.English);
  foreach (MorseCharacterEntry entry in english.Characters)
      Console.WriteLine($"{entry.Character}: {entry.Pattern}");
  ```

  Previously the only public way to obtain a `MorseAlphabet` for a built-in language was
  `MorseAlphabetBuilder.From(Language).Build()`, which repacks the tables at runtime; `ForLanguage` returns the
  same cached instance the fluent chain uses, with no rebuild. `Characters` also works on alphabets built with
  `MorseAlphabetBuilder`, built-in or custom.

  `MorseAlphabet.Prosigns` lists an alphabet's prosigns the same way, as `MorseProsignEntry` values carrying the
  name, the pattern and whether the pattern belongs to something else. The ones that own their pattern, and so
  decode back into brackets, come first.

## 6.0.1

- The logo in the README pointed at a GitHub blob URL, which NuGet's renderer doesn't follow, so it never displayed
  on the package page. It now points at the raw content URL directly.

## 6.0.0

### Added

- **Custom alphabets.** `MorseAlphabetBuilder` lets callers define their own alphabet, or extend and trim a built-in
  one, without forking the library:

  ```csharp
  var klingon = new MorseAlphabetBuilder("Klingon")
      .Add('a', ".-")
      .Add('b', "-...")
      .Build();

  var extended = MorseAlphabetBuilder.From(Language.Deutsch)
      .Add('Ə', "..--.")
      .Remove('$')
      .Build();

  string morse = Morse.GetConverter().ForAlphabet(klingon).ToMorse("ab").Encode();
  ```

  `Add` claims a pattern for decoding, `AddAlias` adds an encode-only character that shares an existing pattern, and
  `Remove` drops a character (promoting an alias to own the pattern if one was waiting). The built-in languages are
  now built through this same builder, so there is one packing path rather than two that could drift apart.

- **Alphabet tables are packed at build time.** A Roslyn source generator reads the `.morse` data files and emits
  the finished lookup tables as `ReadOnlySpan<byte>` blobs, which the compiler stores in the assembly's data section.
  Selecting a language no longer runs the hashing and probing loops at all. The generator shares the packing code
  with the runtime through a linked source file rather than reimplementing it, and a test asserts the two agree for
  every language, so they cannot drift. A malformed table is now a compiler error against the offending line instead
  of an exception the first time that language is used.
- **Languages load independently.** Each sits behind its own nested holder, so touching English no longer runs the
  class initialiser for all eleven.

- **Audio decoding.** `FromAudio` turns received 16-bit PCM back into text, so the library can listen and not only
  send. Goertzel detection measures the energy at the operator's tone per block, Otsu's method finds the level that
  separates key-down from key-up, and the dit length is measured from the signal itself rather than taken on trust
  from the caller. That last part is what lets it follow a drifting hand, a badly stated speed, and Farnsworth
  spacing. Tested by round-tripping through the encoder at speeds from 10 to 40 wpm, tones from 400 Hz to 1.5 kHz,
  and white noise down to 0 dB signal-to-noise.

- **Streaming audio decoding.** `CreateAudioDecoder` returns a `StreamingMorseDecoder` for live input. Push samples
  with `Write`, take characters with `TryRead`, and call `Flush` at the end of a transmission. Chunk sizes are
  arbitrary; anything short of a whole analysis block is carried to the next call. Since it cannot look ahead, both
  the tone threshold and the timing are re-measured from a sliding window, and the opening blocks are held back and
  replayed once there is enough signal to judge them, so the first character is not lost.

- **Prosigns.** Procedural signals such as `<AR>` and `<SK>` are keyed as one unbroken sequence and written in text
  with angle brackets. English ships eight; four own a free pattern and decode back into brackets, and four share a
  pattern with punctuation, so they encode while the punctuation keeps the pattern. `MorseAlphabetBuilder.AddProsign`
  and `AddProsignAlias` add them to any alphabet. Patterns remain capped at 8 symbols, so `SOS` as a single 9-symbol
  signal does not fit; as three letters it is unaffected.

- **Audio output format.** `SetAudioOptions` takes an optional `AudioFormat` covering sample rate, mono or stereo,
  8-bit, 16-bit or 32-bit float, and how long each element fades in and out. Elements now fade over 5 ms by default,
  which removes the click that an instant switch to silence produced at both ends of every dot and dash. Pass
  `EdgeMilliseconds: 0` for the previous behaviour.
- **Elements as a stream.** `PlayAsync` returns the sequence as an `IAsyncEnumerable<MorseElement>` paced in real
  time, and `GetElements` returns it all at once for callers that do their own timing. Each element says whether the
  key is down, which of the five kinds it is, and how long it lasts, so a dot can be told from a dash and the loop
  belongs to the caller: it can await inside it and stop early, neither of which an `Action<bool>` allows. The walk is
  now a pull enumerator rather than a recording pass, so `GetElements` allocates nothing at all and `PlayAsync` costs
  a fixed handful of allocations however long the message is, instead of a task and a timer per element. `DoBlinks` is
  unchanged, and is now a loop over `PlayAsync`.

- **Keyer.** `IambicKeyer` turns paddle presses into correctly timed elements, and `KeyerDecoder` turns what was
  keyed back into text. Holding one contact repeats that element, squeezing both alternates, and a tap on the
  opposite contact during an element is remembered rather than lost. `KeyerMode` picks what a released squeeze does:
  mode A stops after the element in progress, mode B adds one more of the opposite kind. There is no clock inside
  either type, so the keying rules are exercised without waiting for real time, and the caller drives a sidetone or a
  transmitter with whatever timer it already has. Paddles may be polled from a different thread than the one playing.

- **Practice material.** `Koch` generates lessons for the Koch method: random groups drawn from the first N
  characters of the order the method introduces them in, with `Koch.Score` comparing what was copied against what was
  sent and saying whether it clears the ninety per cent needed to unlock the next character. `Callsign` and `Qso`
  produce callsigns and whole contacts in the shape of real traffic, so practice covers the abbreviations, signal
  reports and procedural signals that random groups never teach. All three take an optional `Random`, so a lesson or
  a contact can be reproduced exactly, and `Koch` and `Callsign` have buffer overloads that allocate nothing.

- All enums are byte-backed. `Language` members are renumbered sequentially from 1 to fit, having previously used
  bit-shifted values up to 1024; they were never combinable, so only code persisting the numeric values is affected.

### Breaking changes

- **Exceptions identify the alphabet by name.** `CharacterNotPresentedException.Language` and
  `SequenceNotFoundException.Language` are replaced by `AlphabetName`, because a custom alphabet has no `Language`
  value. Message text is unchanged for the built-in languages.
- `MorseAlphabet` is now public, but opaque: it exposes only `Name`. Obtain one from `MorseAlphabetBuilder`.
- `ICanSpecifyLanguage` gained `ForAlphabet(MorseAlphabet)` alongside `ForLanguage(Language)`.
- **Targets .NET 10** (`net10.0`). .NET 8/9 consumers must stay on 5.x.
- **Audio API.** `GetBytes(out Span<byte>)` handed out a span over a pooled array that had already been returned to
  `ArrayPool<byte>.Shared`, so the caller's audio could be overwritten by any later rent. It is kept as an `[Obsolete]`
  shim that now allocates safely; use the new members instead:
  - `byte[] GetBytes()` – exactly sized array, one allocation.
  - `int GetBytes(Span<byte> destination)` – zero allocation, fills your buffer.
  - `int GetByteCount()` – size the buffer up front.
  - `void WriteTo(Stream destination)` – zero allocation, pooled staging buffer.
- **Timing is now correct** (see fixes below). Generated audio and blink sequences are shorter than in 5.x for the same speeds.
- `ToAudio(string)` / `ToLight(string)` reject Morse strings containing anything other than `.`, `-`, `/` and whitespace
  with an `ArgumentException` instead of silently skipping the symbol.
- `Decode`/`ToMorse`/`ToAudio`/`ToLight` throw `ArgumentException` for an empty string (`ArgumentNullException` for `null`, as before).
- `SetAudioOptions` / `SetBlinkerOptions` throw `ArgumentOutOfRangeException` for a speed of zero or less and for a
  frequency that is not in `(0, 5512.5)` Hz. Previously this produced garbage or crashed later.
- `Language` is no longer marked `[Flags]` (it never was one); values are unchanged.
- The lookup-table types `MorseTable256`, `MorseTableReverse256`, `MorseEntry<,>`, `MorseEntryBuffer256<,>` and the
  `MorseCharacters` class are no longer public. They were implementation details.
- `DoBlinks` gained an optional `CancellationToken` parameter.

### Fixes

- **Farnsworth timing used the wrong constant** (`32` instead of `37.2` seconds per PARIS at 1 wpm). At equal character
  and word speed the gaps were 23 % too long, so the effective speed was lower than requested.
- **Inter-character gaps were inter-word gaps.** The audio and light generators inserted a 7-unit word gap between every
  character and a 14-unit gap between words. Now: 1 unit between symbols, 3 units between characters, 7 units between words.
- **Kurdish `...` (س) could not be decoded** because the hand-written reverse table was missing the entry. Reverse tables
  are now derived from the forward tables so they cannot drift.
- **German `ß` could not be encoded** (`char.ToUpperInvariant('ß')` is still `ß`, not `ẞ`). Both forms are accepted.
- **German parentheses were swapped** relative to ITU and to every other alphabet in the library.
- WAV header and samples are written little-endian on every platform (previously native-endian).
- `Decode` accepts `/` without surrounding spaces, tabs and newlines as separators, and reports the exact offending
  sequence (including sequences longer than 8 symbols, which previously silently overflowed the hash).
- Light blinking no longer accumulates timer drift: every element is scheduled against the wall clock.
- Cancelling a blink sequence switches the light off before the task faults.

### Performance

- The non-ASCII lookup table is sized to the alphabet instead of a fixed 256 slots, which removes the hard ceiling
  that capped how many characters an alphabet could hold, and drops the eleven built-in tables from 11 264 to
  3 728 bytes in total. The hash reduction now keeps the top bits of the Fibonacci hash, so the worst probe length
  across all languages moves only from 2 to 3.
- Zero NuGet dependencies (dropped `CommunityToolkit.HighPerformance` and `ListPool`).
- Alphabet tables are built once per process and shared (previously an 8 KB pair of structs was rebuilt and copied into
  thread-static storage on every language switch).
- Encoding: patterns are stored as 9-bit "tree codes"; ASCII characters are a direct table lookup, other scripts use a
  right-sized hash table with both letter cases pre-inserted (no `ToUpperInvariant` per character). `Encode()` writes
  straight into the result string via `string.Create` – one allocation, no `StringBuilder`.
- Decoding: a sequence is folded into its tree code and decoded with a single array index – no hashing, no string
  comparison. Output goes to a stack buffer (or a pooled one for long inputs) and then into the result string.
- Audio: two passes – count samples, then write every sample exactly once into the destination. One precomputed
  dash-length sine buffer serves dots and dashes; gaps are cleared in place. No intermediate lists, chunks or copies.
  Audio can be produced from text without ever materialising the Morse string.
- Library is marked `IsAotCompatible`; XML documentation and a symbol package are now produced.

## 5.0.0 and earlier

See the Git history.
