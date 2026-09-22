namespace MorseSharp;

/// <summary>
/// The alphabets supported for Morse encoding/decoding.
/// </summary>
/// <remarks>
/// Exactly one language is passed to <see cref="Morse.ForLanguage"/>; these were never combinable. The values are
/// numbered sequentially so the enum fits in a byte, and are stable from 6.0 onwards, so they can be persisted.
/// </remarks>
public enum Language : byte
{
    /// <summary>English (ITU international Morse).</summary>
    English = 1,

    /// <summary>Kurdish, Arabic script (Sorani).</summary>
    Kurdish = 2,

    /// <summary>Kurdish, Latin script (Hawar).</summary>
    KurdishLatin = 3,

    /// <summary>Arabic.</summary>
    Arabic = 4,

    /// <summary>German (Deutsch).</summary>
    Deutsch = 5,

    /// <summary>Spanish (Español).</summary>
    Spanish = 6,

    /// <summary>French (Français).</summary>
    French = 7,

    /// <summary>Italian (Italiano).</summary>
    Italian = 8,

    /// <summary>Japanese (Wabun code, katakana).</summary>
    Japanese = 9,

    /// <summary>Portuguese (Português).</summary>
    Portugues = 10,

    /// <summary>Russian (Cyrillic).</summary>
    Russian = 11,
}
