namespace MorseSharp;

/// <summary>
/// Describes the languages for morse endcoding/decoding.
/// </summary>

[Flags]
public enum Language
{
    /// <summary>
    /// English Language.
    /// </summary>
    English = 1,
    /// <summary>
    /// Kurdish Language.
    /// </summary>
    Kurdish = 1 << 1,
    /// <summary>
    /// Kurdish Language.
    /// </summary>
    KurdishLatin = 1 << 2,
    /// <summary>
    /// Arabic Language.
    /// </summary>
    Arabic = 1 << 3,
    /// <summary>
    /// Deutsch Language.
    /// </summary>
    Deutsch = 1 << 4,
    /// <summary>
    /// Espaneol Language.
    /// </summary>
    Espanol = 1 << 5,
    /// <summary>
    /// Francais Language.
    /// </summary>
    Francais = 1 << 6,
    /// <summary>
    /// Italiano Language.
    /// </summary>
    Italiano = 1 << 7,
    /// <summary>
    /// Japanese Language.
    /// </summary>
    Japanese = 1 << 8,
    /// <summary>
    /// Portugues Language.
    /// </summary>
    Portugues = 1 << 9,
    /// <summary>
    /// Russian Language.
    /// </summary>
    Russian = 1 << 10,

}