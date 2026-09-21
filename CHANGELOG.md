# Changelog

## 6.0.0

### Breaking changes

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

- Zero NuGet dependencies (dropped `CommunityToolkit.HighPerformance` and `ListPool`).
- Alphabet tables are built once per process and shared (previously an 8 KB pair of structs was rebuilt and copied into
  thread-static storage on every language switch).
- Encoding: patterns are stored as 9-bit "tree codes"; ASCII characters are a direct table lookup, other scripts use a
  256-slot hash table with both letter cases pre-inserted (no `ToUpperInvariant` per character). `Encode()` writes
  straight into the result string via `string.Create` – one allocation, no `StringBuilder`.
- Decoding: a sequence is folded into its tree code and decoded with a single array index – no hashing, no string
  comparison. Output goes to a stack buffer (or a pooled one for long inputs) and then into the result string.
- Audio: two passes – count samples, then write every sample exactly once into the destination. One precomputed
  dash-length sine buffer serves dots and dashes; gaps are cleared in place. No intermediate lists, chunks or copies.
  Audio can be produced from text without ever materialising the Morse string.
- Library is marked `IsAotCompatible`; XML documentation and a symbol package are now produced.

## 5.0.0 and earlier

See the Git history.
