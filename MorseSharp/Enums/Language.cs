namespace MorseSharp;

/// <summary>
/// The alphabets supported for Morse encoding/decoding.
/// </summary>
/// <remarks>
/// The numeric values are kept stable between releases so they can be persisted safely.
/// The enum is not a flags enum: exactly one language must be passed to <see cref="Morse.ForLanguage"/>.
/// </remarks>
public enum Language
{
    /// <summary>English (ITU international Morse).</summary>
    English = 1,
    /// <summary>Kurdish, Arabic script (Sorani).</summary>
    Kurdish = 1 << 1,
    /// <summary>Kurdish, Latin script (Hawar).</summary>
    KurdishLatin = 1 << 2,
    /// <summary>Arabic.</summary>
    Arabic = 1 << 3,
    /// <summary>German (Deutsch).</summary>
    Deutsch = 1 << 4,
    /// <summary>Spanish (Español).</summary>
    Spanish = 1 << 5,
    /// <summary>French (Français).</summary>
    French = 1 << 6,
    /// <summary>Italian (Italiano).</summary>
    Italian = 1 << 7,
    /// <summary>Japanese (Wabun code, katakana).</summary>
    Japanese = 1 << 8,
    /// <summary>Portuguese (Português).</summary>
    Portugues = 1 << 9,
    /// <summary>Russian (Cyrillic).</summary>
    Russian = 1 << 10,
}
