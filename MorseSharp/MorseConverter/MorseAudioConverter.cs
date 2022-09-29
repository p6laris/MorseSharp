using MorseCodeToAudio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MorseSharp.MorseConverter
{

    /// <summary>
    /// <para>
    /// Converts morse code to a wav format audio of dash and dots.
    /// This class is a wrapper of <see href="https://github.com/jstoddard">jstoddard</see>'s <see href="https://github.com/jstoddard/CWLibrary">CWLibrary</see> Library.
    /// </para>
    /// </summary>
    public class MorseAudioConverter : TextToMorse
    {
        /// <summary>
        /// initializes an instance of type <see cref="MorseConverter.MorseAudioConverter"/> with custom audio configuration.
        /// </summary>
        /// <param name="charSpeed">Character speed in WPM</param>
        /// <param name="wordSpeed">Overall speed in WPM (must be <= character speed)</param>
        /// <param name="frequency">Tone frequency</param>
        public MorseAudioConverter(int charSpeed, int wordSpeed, double frequency) : base(charSpeed, wordSpeed, frequency){}
        /// <summary>
        /// initializes an instance of type <see cref="MorseConverter.MorseAudioConverter"/> with custom audio configuration.
        /// </summary>
        /// <param name="charSpeed">Character speed in WPM</param>
        /// <param name="wordSpeed">Overall speed in WPM (must be <= character speed)</param>
        public MorseAudioConverter(int charSpeed, int wordSpeed) : this(charSpeed, wordSpeed, 600.0) { }
        /// <summary>
        /// initializes an instance of type <see cref="MorseConverter.MorseAudioConverter"/> with custom audio configuration.
        /// </summary>
        /// <param name="charSpeed">Character speed in WPM</param>
        public MorseAudioConverter(int charSpeed) : this(charSpeed, charSpeed) { }
        /// <summary>
        /// initializes an instance of type <see cref="MorseConverter.MorseAudioConverter"/> with custom audio configuration.
        /// </summary>
        public MorseAudioConverter() : this(20) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<byte[]> ConvertMorseToAudio(string text)
        {
            if(text is not null)
            {
                Memory<byte> audioByte = ConvertToMorse(text).AsMemory();
                return await Task.Run(() => audioByte.ToArray());
            }
            throw new ArgumentNullException(nameof(text));
        }


    }
}
