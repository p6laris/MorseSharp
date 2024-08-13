namespace MorseSharp;


/// <summary>
/// Provides functionality to decode/encode Morse code, convert text to Morse code,
/// generate audio bytes, and control light blinking based on Morse code.
/// </summary>
public sealed class Morse : ICanSpecifyLanguage, ICanSetConversionOption,
    ICanConvertToAudio, ICanSetAudioOptions, ICanGenerateAudioAndLight,
    ICanSetBlinkerOptions, ICanConvertToLight
{

    private static ThreadLocal<Lazy<Dictionary<char, string>>> _morseChar;
    private static ThreadLocal<Language> _sLanguage;
    private static ThreadLocal<StringBuilder> _strBuilder;
    private static ThreadLocal<int> _charSpeed;
    private static ThreadLocal<int> _wordSpeed;
    private static ThreadLocal<double> _frequency;

    private const Language NonLatin = Language.Kurdish | Language.Arabic;

    private static Morse? _instance;
    private static object ObjLock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Morse"/> class.
    /// </summary>
    private Morse()
    {
        _sLanguage = new ThreadLocal<Language>(() => default(Language));
        _morseChar = new ThreadLocal<Lazy<Dictionary<char, string>>>(() => new Lazy<Dictionary<char, string>>());
        _strBuilder = new ThreadLocal<StringBuilder>(() => new StringBuilder());
        _charSpeed = new ThreadLocal<int>(() => 0);
        _wordSpeed = new ThreadLocal<int>(() => 0);
        _frequency = new ThreadLocal<double>(() => 0.0);

    }

    /// <summary>
    /// Gets an instance of the <see cref="Morse"/> class for conversion.
    /// </summary>
    /// <returns>An instance of the <see cref="ICanSpecifyLanguage"/> interface.</returns>

    public static ICanSpecifyLanguage GetConverter()
    {
        if (_instance is null)
        {
            lock (ObjLock)
            {
                if (_instance is null)
                {
                    _instance = new Morse();
                }

            }
        }

        return _instance;
    }

    /// <summary>
    /// Specify the language for Morse code conversion.
    /// </summary>
    /// <param name="language">The language for which to set conversion options.</param>
    /// <returns>An instance of the <see cref="ICanSetConversionOption"/> interface.</returns>
    public ICanSetConversionOption ForLanguage(Language language)
    {

        //cache the language
        if (language != _sLanguage.Value)
        {
            _sLanguage.Value = language;
            _morseChar.Value = new Lazy<Dictionary<char, string>>(MorseCharacters.GetLanguageCharacter(_sLanguage.Value)!);
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


        if (_strBuilder.Value!.Length > 0)
            _strBuilder.Value.Clear();

        int count = 0;

        unsafe
        {
            fixed (char* ptr = morse)
            {
                // Count occurrences of spaces to split Morse code into words
                count = StringCounter.CountOccurrences(ptr, ' ');
            }

        }

        Range[]? rngArray = null;
        Span<Range> splitedRange = count + 1 < 256
            ? stackalloc Range[count + 1] : rngArray = ArrayPool<Range>.Shared.Rent(count + 1);

        // Split Morse code into individual words
        morse.AsSpan().Split(splitedRange, ' ', StringSplitOptions.None);

        for (int i = 0; i < splitedRange.Length; i++)
        {
            var sequence = morse.AsSpan().Slice(splitedRange[i].Start.Value, splitedRange[i].End.Value - splitedRange[i].Start.Value);

            if (sequence.IsWhiteSpace())
                continue;

            if (sequence != "/")
            {
                var value = _morseChar.Value!.Value.KeyByValue(sequence.ToString());

                if (value != '\0')
                    _strBuilder.Value.Append(value);

                else
                    throw new SequenceNotFoundException(sequence.ToString(), language: _sLanguage.Value);
            }
        }

        if (rngArray is not null)
            ArrayPool<Range>.Shared.Return(rngArray);

        return _strBuilder.Value.ToString();
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

        if (_strBuilder.Value!.Length > 0)
            _strBuilder.Value.Clear();


        var length = text.Length;
        char[]? txtArray = null;
        Span<char> txt = length < 256
            ? stackalloc char[length] : txtArray = ArrayPool<char>.Shared.Rent(length);


        if ((_sLanguage.Value & NonLatin) == 0)
            text.AsSpan().ToUpperInvariant(txt);
        else
            text.AsSpan().CopyTo(txt);



        for (int i = 0; i < length; i++)
        {
            if (_morseChar.Value!.Value.TryGetValue(txt[i], out string? val))
            {
                // Check if it's not the last character before appending a space
                if (i < text.Length - 1)
                    _strBuilder.Value.Append(val.AsSpan()).Append(' ');

                else
                    _strBuilder.Value.Append(val.AsSpan());
            }
            else
                throw new CharacterNotPresentedException(txt[i], language: _sLanguage.Value);
        }

        if (txtArray is not null)
            ArrayPool<char>.Shared.Return(txtArray);

        return this;

    }


    /// <summary>
    /// Generate audio bytes based on the set options.
    /// </summary>
    /// <param name="destination">The destination span to store the audio bytes.</param>
    public void GetBytes(out Span<byte> destination)
    {
        AudioConverter a = new AudioConverter(_charSpeed.Value, _wordSpeed.Value, _frequency.Value);
        a.ConvertToAudio(_strBuilder.Value!.ToString().AsSpan(), out Span<byte> bytes);
        destination = bytes;
    }

    /// <summary>
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

        _charSpeed.Value = charSpeed;
        _wordSpeed.Value = wordSpeed;
        _frequency.Value = frequency;

        return this;
    }


    /// <summary>
    /// Encode the Morse code.
    /// </summary>
    /// <returns>The encoded Morse code as a string.</returns>
    public string Encode() => _strBuilder.Value!.ToString();


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


        if (_strBuilder.Value!.Length > 0)
            _strBuilder.Value.Length = 0;

        _strBuilder.Value.Append(morse);
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


        if (_strBuilder.Value!.Length > 0)
            _strBuilder.Value.Length = 0;

        _strBuilder.Value.Append(morse);
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

        _charSpeed.Value = charSpeed;
        _wordSpeed.Value = wordSpeed;

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

        LightBlinker lightBlinker = new LightBlinker(_charSpeed.Value, _wordSpeed.Value, blinkerAction);
        await lightBlinker.BlinkLight(_strBuilder.Value!.ToString());
    }
}