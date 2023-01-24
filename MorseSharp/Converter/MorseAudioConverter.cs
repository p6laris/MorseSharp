using MorseSharp.Audio;

namespace MorseSharp.Converter
{

    /// <summary>
    /// <para>
    /// Converts morse code to a wav format audio of dash and dots.
    /// This class is a wrapper of <see href="https://github.com/jstoddard">jstoddard</see>'s <see href="https://github.com/jstoddard/CWLibrary">CWLibrary</see> Library.
    /// </para>
    /// </summary>
    public class MorseAudioConverter : AudioConverter
    {
        /// <summary>
        /// initializes an instance of type <see cref="Converter.MorseAudioConverter"/> with custom audio configuration.
        /// </summary>
        /// <param name="language">An enum of type <see cref="Language"></see>to specifies the language</param>
        /// <param name="charSpeed">Character speed in WPM</param>
        /// <param name="wordSpeed">Overall speed in WPM (must be <= character speed)</param>
        /// <param name="frequency">Tone frequency</param>
        public MorseAudioConverter(Language language,int charSpeed, int wordSpeed, double frequency) : base(language,charSpeed, wordSpeed, frequency){}
        /// <summary>
        /// initializes an instance of type <see cref="Converter.MorseAudioConverter"/> with custom audio configuration.
        /// </summary>
        /// <param name="language">An enum of type <see cref="Language"></see>to specifies the language</param>
        /// <param name="charSpeed">Character speed in WPM</param>
        /// <param name="wordSpeed">Overall speed in WPM (must be <= character speed)</param>
        public MorseAudioConverter(Language language, int charSpeed, int wordSpeed) : base(language,charSpeed, wordSpeed, 600.0) { }
        /// <summary>
        /// initializes an instance of type <see cref="Converter.MorseAudioConverter"/> with custom audio configuration.
        /// </summary>
        /// <param name="language">An enum of type <see cref="Language"></see>to specifies the language</param>
        /// <param name="charSpeed">Character speed in WPM</param>
        public MorseAudioConverter(Language language,int charSpeed) : base(language,charSpeed, charSpeed) { }
        /// <summary>
        /// initializes an instance of type <see cref="Converter.MorseAudioConverter"/> with custom audio configuration.
        /// <param name="language">An enum of type <see cref="Language"></see>to specifies the language</param>
        /// </summary>
        public MorseAudioConverter(Language language) : base(language,20) { }
        /// <summary>
        /// initializes an instance of type <see cref="Converter.MorseAudioConverter"/> with custom audio configuration.
        /// </summary>
        public MorseAudioConverter() : base(Language.English){}

        /// <summary>
        /// Generate's audio for the given text.
        /// </summary>
        /// <param name="text">Text to generate audio for.</param>
        /// <returns><see cref="byte[]"/> of the generated audio.</returns>
        /// <exception cref="ArgumentNullException">Throws if the text parameter was null.</exception>
        public async Task<byte[]> ConvertMorseToAudio(string text)
        {
            if(text is not null)
            {
                Memory<byte> audioByte = ConvertToMorse(text);
                return await Task.Run(() => audioByte.ToArray());
            }
            throw new ArgumentNullException(nameof(text));
        }


    }
}
