# Changelog

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
