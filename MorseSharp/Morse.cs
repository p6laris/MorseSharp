namespace MorseSharp;


/// <summary>
/// Decode/encodes morse text.
/// </summary>
public sealed class Morse : ICanSpecifyLanguage, ICanSetConversionOption,
    ICanConvertToAudio, ICanSetAudioOptions, ICanGenerateAudioAndLight,
    ICanSetBlinkerOptions, ICanConvertToLight
{
    private Lazy<Dictionary<char, string>> _morseChar = default!;

    private Language _sLanguage;
    private const Language NonLatin = Language.Kurdish | Language.Arabic;
    private static Morse? _instance;
    private static readonly StringBuilder _strBuilder = new StringBuilder();
    private static readonly object ObjLock = new();
    private int _charSpeed;
    private int _wordSpeed;
    private double _frequency;

    /// <summary>
    /// Initializes a new instance of the <see cref="Morse"/> class.
    /// </summary>
    private Morse()
    {
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
                _instance = new Morse();
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
        if (language != _sLanguage)
        {
            _sLanguage = language;
            _morseChar = new Lazy<Dictionary<char, string>>(MorseCharacters.GetLanguageCharacter(language)!);
        }

        return this;
    }

    /// <summary>
    /// Convert Morse code to text.
    /// </summary>
    /// <param name="morse">The Morse code to convert to text.</param>
    /// <returns>The converted text.</returns>
    public string Decode(string morse)
    {
        if (!string.IsNullOrEmpty(morse))
        {

            if (_strBuilder.Length > 0)
                _strBuilder.Length = 0;


            ReadOnlySpan<char> sMorse = morse.AsSpan();

            int count = 0;

            unsafe
            {
                fixed (char* ptr = morse)
                {
                    // Count occurrences of spaces to split Morse code into words
                    count = StringCounter.CountOccurrences(ptr, ' ');
                }

            }

            Span<Range> splitedRange = new Span<Range>(new Range[count + 1]);

            // Split Morse code into individual words
            sMorse.Split(splitedRange, ' ', StringSplitOptions.None);

            for (int i = 0; i < splitedRange.Length; i++)
            {
                var sequence = sMorse.Slice(splitedRange[i].Start.Value, splitedRange[i].End.Value - splitedRange[i].Start.Value);

                if (sequence.IsWhiteSpace())
                    continue;

                if (sequence != "/")
                {
                    if (_morseChar.Value.Values.Contains(sequence.ToString()))
                    {
                        var value = _morseChar.Value.KeyByValue(sequence.ToString());
                        _strBuilder.Append(value);
                    }
                    else
                        throw new SequenceNotFoundException(sequence.ToString(), language: _sLanguage);
                }
            }
            return _strBuilder.ToString();
        }
        else
            throw new ArgumentNullException(nameof(morse));



    }

    /// <summary>
    /// Convert text to Morse code.
    /// </summary>
    /// <param name="text">The text to convert to Morse code.</param>
    /// <returns>The Morse code representation of the text.</returns>
    public ICanGenerateAudioAndLight ToMorse(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {

            if (_strBuilder.Length > 0)
                _strBuilder.Length = 0;


            ReadOnlySpan<char> sText = text.AsSpan();


            var length = sText.Length;


            if ((_sLanguage & NonLatin) == 0)
            {
                unsafe
                {
                    fixed (char* ptr = text)
                    fixed (char* outPtr = sText)
                    {
                        // Convert text to uppercase using pointers
                        ToUpperSpanExtention.ToUpperWithPointers(ptr, outPtr, text.Length);
                    }
                }
            }

            for (int i = 0; i < length; i++)
            {
                if (_morseChar.Value.TryGetValue(sText[i], out string? val))
                {
                    // Check if it's not the last character before appending a space
                    if (i < text.Length - 1)
                    {
                        _strBuilder.Append(val.AsSpan()).Append(' ');
                    }
                    else
                        _strBuilder.Append(val.AsSpan());
                }
                else
                    throw new WordNotPresentedException(sText[i], language: _sLanguage);
            }

            return this;
        }
        else
            throw new ArgumentException(nameof(text));

    }


    /// <summary>
    /// Generate audio bytes based on the set options.
    /// </summary>
    /// <param name="destination">The destination span to store the audio bytes.</param>
    public void GetBytes(out Span<byte> destination)
    {
        AudioConverter a = new AudioConverter(_charSpeed, _wordSpeed, _frequency);
        a.ConvertToAudio(_strBuilder.ToString(), out Span<byte> bytes);
        destination = bytes;
    }

    /// <summary>
    /// Set audio conversion options.
    /// </summary>
    /// <param name="charSpeed">The character speed for audio.</param>
    /// <param name="wordSpeed">The word speed for audio.</param>
    /// <param name="frequency">The frequency for audio.</param>
    /// <returns>An instance of the <see cref="ICanConvertToAudio"/> interface.</returns>
    public ICanConvertToAudio SetAudioOptions(int charSpeed = 25, int wordSpeed = 25, double frequency = 700)
    {
        if (charSpeed < wordSpeed)
            throw new SmallerWordSpeedException(charSpeed, wordSpeed);

        _charSpeed = charSpeed;
        _wordSpeed = wordSpeed;
        _frequency = frequency;

        return this;
    }

    public string Encode() => _strBuilder.ToString();

    public ICanSetAudioOptions ToAudio() => this;

    public ICanSetBlinkerOptions ToLight() => this;

    public ICanConvertToLight SetBlinkerOptions(int charSpeed = 25, int wordSpeed = 25)
    {
        if (charSpeed < wordSpeed)
            throw new SmallerWordSpeedException(charSpeed, wordSpeed);

        _charSpeed = charSpeed;
        _wordSpeed = wordSpeed;

        return this;
    }

    public async Task DoBlinks(Action<bool> blinkerAction)
    {
        if (blinkerAction == null)
            throw new ArgumentNullException(nameof(blinkerAction));

        LightBlinker lightBlinker = new LightBlinker(_charSpeed, _wordSpeed, blinkerAction);
        await lightBlinker.BlinkLight(_strBuilder.ToString());
    }
}
