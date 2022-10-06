using MorseSharp.Audio.Chunks;
using MorseSharp.Audio.Languages;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseSharp.Audio
{
    public class AudioConverter
    {
        // Character speed in WPM
        private int CharacterSpeed { get; set; }
        // Overall speed in WPM (must be <= character speed)
        private int WordSpeed { get; set; }
        // Tone frequency
        private double Frequency { get; set; }

        //Language
        private Language Language { get; set; }
        public AudioConverter(Language language, int charSpeed, int wordSpeed, double frequency)
        {
            CharacterSpeed = charSpeed;
            WordSpeed = wordSpeed;
            Frequency = frequency;
            Language = language;
        }

        public AudioConverter(Language language, int charSpeed, int wordSpeed) : this(language,charSpeed, wordSpeed, 600.0) { }
        public AudioConverter(Language language, int wpm) : this(language,wpm, wpm) { }
        public AudioConverter(Language language) : this(language,20) { }
        public AudioConverter() : this(Language.English) { }

        // Return given number of seconds of sine wave
        private Span<short> GetWave(double seconds)
        {
            Span<short> waveArray;
            int samples = (int)(11025 * seconds);

            waveArray = new short[samples];

            for (int i = 0; i < samples; i++)
            {
                waveArray[i] = Convert.ToInt16(32760 * Math.Sin(i * 2 * Math.PI * Frequency / 11025));
            }

            return waveArray;
        }

        // Return given number of seconds of flatline. This could also be
        // achieved with slnt chunks inside a wavl chunk, but the resulting
        // file might not be universally readable. If saving space is that
        // important, it would be better to compress the output as mp3 or ogg
        // anyway.
        private Span<short> GetSilence(double seconds)
        {
            Span<short> waveArray;
            int samples = (int)(11025 * seconds);

            waveArray = new short[samples];

            return waveArray;
        }

        // Dot -- 1 unit long
        private Span<short> GetDot()
        {
            return GetWave(1.2 / CharacterSpeed);
        }

        // Dash -- 3 units long
        private Span<short> GetDash()
        {
            return GetWave(3.6 / CharacterSpeed);
        }

        // Inter-element space -- 1 unit long
        private Span<short> GetInterEltSpace()
        {
            return GetSilence(1.2 / CharacterSpeed);
        }

        // Space between letters -- nominally 3 units, but adjusted for
        // Farnsworth timing (if word speed is lower than character
        // speed) based on ARRL's Morse code timing standard:
        // http://www.arrl.org/files/file/Technology/x9004008.pdf
        private Span<short> GetInterCharSpace()
        {
            double delay = (60.0 / WordSpeed) - (32.0 / CharacterSpeed);
            double spaceLength = 3 * delay / 19;
            return GetSilence(spaceLength);
        }

        // Space between words -- nominally 7 units, but adjusted for
        // Farnsworth timing in case word speed is lower than character
        // speed.
        private Span<short> GetInterWordSpace()
        {
            double delay = (60.0 / WordSpeed) - (32.0 / CharacterSpeed);
            double spaceLength = 7 * delay / 19;
            return GetSilence(spaceLength);
        }

        // Return a single character as a waveform
        private Span<short> GetCharacter(string character)
        {
            Span<short> space = GetInterEltSpace();
            Span<short> dot = GetDot();
            Span<short> dash = GetDash();
            List<short> morseChar = new List<short>();

            string morseSymbol = string.Empty;

            if (Language == Language.English)
            {
                if (MorseCharacters.GetMorseCharactersEnglish().ContainsKey(character[0]))
                    morseSymbol = MorseCharacters.GetMorseCharactersEnglish()[character[0]];
                else
                    throw new KeyNotFoundException($"{character} is not presented in english language.");
                
            }

            else
            {
                if (MorseCharacters.GetMorseCharactersKurdish().ContainsKey(character[0]))
                    morseSymbol = MorseCharacters.GetMorseCharactersKurdish()[character[0]];
                else
                    throw new KeyNotFoundException($"{character} is not presented in kurdish language.");
            }
                
           
            for (int i = 0; i < morseSymbol.Length; i++)
            {
                if (i > 0)
                    morseChar.AddRange(space.ToArray());
                if (morseSymbol[i] == '_')
                    morseChar.AddRange(dash.ToArray());
                else if (morseSymbol[i] == '.')
                    morseChar.AddRange(dot.ToArray());
            }

            return morseChar.ToArray<short>();
        }

        // Return a word as a waveform
        private Span<short> GetWord(string word)
        {
            List<short> data = new List<short>();

            for (int i = 0; i < word.Length; i++)
            {
                if (i > 0)
                    data.AddRange(GetInterCharSpace().ToArray());
                if (word[i] == '<')
                {
                    // Prosign
                    int end = word.IndexOf('>', i);
                    if (end < 0)
                        throw new ArgumentException();
                    data.AddRange(GetCharacter(word.Substring(i, end + 1 - i)).ToArray());
                    i = end;
                }
                else
                {
                    data.AddRange(GetCharacter(word[i].ToString()).ToArray());
                }
            }

            return data.ToArray<short>();
        }

        // Return a string (lower case text only, unrecognized characters
        // throw an exception -- see Characters.cs for the list of recognized
        // characters) as a waveform wrapped in a DataChunk, ready to by added
        // to a wave file.
        private DataChunk GetText(string text)
        {
            List<short> data = new List<short>();

            string[] words = text.Split(' ');

            for (int i = 0; i < words.Length; i++)
            {
                if (i > 0)
                    data.AddRange(GetInterWordSpace().ToArray());
                data.AddRange(GetWord(words[i]).ToArray());
            }

            // Pad the end with a little bit of silence. Otherwise the last
            // character may sound funny in some media players.
            data.AddRange(GetInterCharSpace().ToArray());

            DataChunk dataChunk = new DataChunk(data.ToArray<short>());

            return dataChunk;
        }

        // Returns a byte array in the Wave file format containing the given
        // text in morse code
        internal Memory<byte> ConvertToMorse(string text)
        {
            DataChunk data = default!;
            if(Language == Language.English)
                data = GetText(text.ToUpper());
            else
                data = GetText(text);
            
            FormatChunk formatChunk = new FormatChunk();
            HeaderChunk headerChunk = new HeaderChunk(formatChunk, data);
            return headerChunk.ToBytes();
        }
    }
}
