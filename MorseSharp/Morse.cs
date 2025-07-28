namespace MorseSharp;


/// <summary>
/// Provides functionality to decode/encode Morse code, convert text to Morse code,
/// generate audio bytes, and control light blinking based on Morse code.
/// </summary>
public sealed class Morse : ICanSpecifyLanguage, ICanSetConversionOption,
    ICanConvertToAudio, ICanSetAudioOptions, ICanGenerateAudioAndLight,
    ICanSetBlinkerOptions, ICanConvertToLight
{
    private static Morse? _instance;

    [ThreadStatic] private static MorseTable256 _morseChar;
    [ThreadStatic] private static MorseTableReverse256 _morseCharReversed;
    [ThreadStatic] private static Language _sLanguage;
    [ThreadStatic] private static int _charSpeed;
    [ThreadStatic] private static int _wordSpeed;
    [ThreadStatic] private static double _frequency;
    [ThreadStatic] private static StringBuilder? _builder;

    private static StringBuilder _strBuilder
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _builder ??= new StringBuilder();
    }

    /// <summary>
    /// Gets an instance of the <see cref="Morse"/> class for conversion.
    /// </summary>
    /// <returns>An instance of the <see cref="ICanSpecifyLanguage"/> interface.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ICanSpecifyLanguage GetConverter() => _instance ??= new Morse();

    /// <summary>
    /// Specify the language for Morse code conversion.
    /// </summary>
    /// <param name="language">The language for which to set conversion options.</param>
    /// <returns>An instance of the <see cref="ICanSetConversionOption"/> interface.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ICanSetConversionOption ForLanguage(Language language)
    {
        // Cache the language
        if (language == default || language != _sLanguage)
        {
            _sLanguage = language;
            _morseChar = MorseCharacters.GetLanguageCharacter(language);
            _morseCharReversed = MorseCharacters.GetLanguageCharacterReversed(language);
        }

        return this;
    }

    /// <summary>
    /// Convert Morse code to text.
    /// </summary>
    /// <param name="morse">The Morse code to convert to text.</param>
    /// <returns>The converted text.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input Morse code is null or empty.</exception>
    /// <exception cref="SequenceNotFoundException">
    /// Thrown when an invalid Morse code sequence is encountered, and the corresponding character cannot be found.
    /// </exception>
    public string Decode(string morse)
    {
        if (string.IsNullOrEmpty(morse))
            throw new ArgumentNullException(nameof(morse));

        if (_strBuilder.Length > 0)
            _strBuilder.Clear();

        var morseSpan = morse.AsSpan();

        foreach (var range in morseSpan.Split(' '))
        {
            var chars = morseSpan[range];

            if (chars.IsWhiteSpace())
                continue;

            if (_morseCharReversed.TryGetValue(chars, out var value))
                _strBuilder.Append(value);
            else
                throw new SequenceNotFoundException(chars, language: _sLanguage);
        }

        return _strBuilder.ToString();
    }

    /// <summary>
    /// Convert text to Morse code.
    /// </summary>
    /// <param name="text">The text to convert to Morse code.</param>
    /// <returns>The Morse code representation of the text.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input text is null or empty.</exception>
    /// <exception cref="CharacterNotPresentedException">
    /// Thrown when a character in the input text does not have a corresponding Morse code representation.
    /// </exception>

    public ICanGenerateAudioAndLight ToMorse(string text)
    {
        if (string.IsNullOrEmpty(text))
            throw new ArgumentNullException(nameof(text));

        if (_strBuilder.Length > 0)
            _strBuilder.Clear();

        var span = text.AsSpan();
        int lastCharIndex = span.Length - 1;

        for (int i = 0; i < span.Length; i++)
        {
            char ch = span[i];
            if (!_morseChar.TryGetValue(ch, out var morse))
                throw new CharacterNotPresentedException(ch, language: _sLanguage);

            _strBuilder.Append(morse);

            // Append space except last character
            if (i < lastCharIndex)
                _strBuilder.Append(' ');
        }

        return this;
    }


    /// <summary>
    /// Generate audio bytes based on the set options.
    /// </summary>
    /// <param name="destination">The destination span to store the audio bytes.</param>
    public void GetBytes(out Span<byte> destination)
    {
        using AudioConverter converter = new AudioConverter(_charSpeed, _wordSpeed, _frequency);
        converter.ConvertToAudio(_strBuilder!.ToString().AsSpan(), out Span<byte> bytes);
        destination = bytes;
    }

    /// <summary>
    ///
    /// Set audio conversion options.
    /// </summary>
    /// <param name="charSpeed">The character speed for audio.</param>
    /// <param name="wordSpeed">The word speed for audio.</param>
    /// <param name="frequency">The frequency for audio.</param>
    /// <returns>An instance of the <see cref="ICanConvertToAudio"/> interface.</returns>
    /// <exception cref="SmallerCharSpeedException">
    /// Thrown when the character speed is less than the word speed, which is invalid for audio conversion.
    /// </exception>
    public ICanConvertToAudio SetAudioOptions(int charSpeed = 25, int wordSpeed = 25, double frequency = 700)
    {
        if (charSpeed < wordSpeed)
            throw new SmallerCharSpeedException(charSpeed, wordSpeed);

        _charSpeed = charSpeed;
        _wordSpeed = wordSpeed;
        _frequency = frequency;

        return this;
    }


    /// <summary>
    /// Encode the Morse code.
    /// </summary>
    /// <returns>The encoded Morse code as a string.</returns>
    public string Encode() => _strBuilder.ToString();


    /// <summary>
    /// Switch to audio conversion mode.
    /// </summary>
    /// <returns>An instance of the <see cref="ICanSetAudioOptions"/> interface.</returns>
    public ICanSetAudioOptions ToAudio() => this;

    /// <summary>
    /// Switch to light blinking mode.
    /// </summary>
    /// <returns>An instance of the <see cref="ICanSetBlinkerOptions"/> interface.</returns>
    public ICanSetBlinkerOptions ToLight() => this;

    /// <summary>
    /// Switch to audio conversion mode with a specified Morse code.
    /// </summary>
    /// <param name="morse">The Morse code to convert to audio.</param>
    /// <returns>An instance of the <see cref="ICanSetAudioOptions"/> interface.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input Morse code is null or empty.</exception>
    public ICanSetAudioOptions ToAudio(string morse)
    {
        if (string.IsNullOrEmpty(morse))
            throw new ArgumentNullException(nameof(morse));


        if (_strBuilder.Length > 0)
            _strBuilder.Length = 0;

        _strBuilder.Append(morse);
        return this;
    }

    /// <summary>
    /// Switch to light blinking mode with a specified Morse code.
    /// </summary>
    /// <param name="morse">The Morse code to convert to light blinking.</param>
    /// <returns>An instance of the <see cref="ICanSetBlinkerOptions"/> interface.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the input Morse code is null or empty.</exception>

    public ICanSetBlinkerOptions ToLight(string morse)
    {
        if (string.IsNullOrEmpty(morse))
            throw new ArgumentNullException(nameof(morse));


        if (_strBuilder.Length > 0)
            _strBuilder.Length = 0;

        _strBuilder.Append(morse);
        return this;
    }

    /// <summary>
    /// Set light blinking options.
    /// </summary>
    /// <param name="charSpeed">The character speed for light blinking.</param>
    /// <param name="wordSpeed">The word speed for light blinking.</param>
    /// <returns>An instance of the <see cref="ICanConvertToLight"/> interface.</returns>
    /// <exception cref="SmallerCharSpeedException">
    /// Thrown when the character speed is less than the word speed, which is invalid for light blinking.
    /// </exception>
    public ICanConvertToLight SetBlinkerOptions(int charSpeed = 25, int wordSpeed = 25)
    {
        if (charSpeed < wordSpeed)
            throw new SmallerCharSpeedException(charSpeed, wordSpeed);

        _charSpeed = charSpeed;
        _wordSpeed = wordSpeed;

        return this;
    }

    /// <summary>
    /// Perform light blinking based on the set options.
    /// </summary>
    /// <param name="blinkerAction">The action to perform for each blink (true for on, false for off).</param>
    /// <returns>An asynchronous task.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the blinker action is null.</exception>
    public async Task DoBlinks(Action<bool> blinkerAction)
    {
        if (blinkerAction is null)
            throw new ArgumentNullException(nameof(blinkerAction));

        LightBlinker lightBlinker = new LightBlinker(_charSpeed, _wordSpeed, blinkerAction);
        await lightBlinker.BlinkLight(_strBuilder.ToString());
    }
}
