using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MorseSharp.Alphabet;

namespace MorseSharp.Generators;

/// <summary>
/// Reads a <c>.morse</c> file.
/// </summary>
/// <remarks>
/// The format is deliberately dull, because the compiler runs this unattended on every keystroke in the IDE:
/// <code>
/// # comment
/// [PRIMARY]
/// A&#9;.-
/// [ALIAS]
/// ß&#9;......
/// </code>
/// One entry per line, the character and its pattern separated by a tab. A character that would confuse the parser
/// can be written as <c>\uXXXX</c>.
/// </remarks>
internal static class MorseFileParser
{
    /// <summary>Parses the text of one file.</summary>
    /// <param name="name">The alphabet name, taken from the file name.</param>
    /// <param name="filePath">Path of the file, for diagnostics.</param>
    /// <param name="text">The file contents.</param>
    public static ParsedAlphabet Parse(string name, string filePath, string text)
    {
        List<MorseEntry> entries = new List<MorseEntry>();
        List<AlphabetError> errors = new List<AlphabetError>();

        bool isAlias = false;
        bool sawSection = false;
        int lineNumber = -1;

        using (StringReader reader = new StringReader(text))
        {
            string? raw;
            while ((raw = reader.ReadLine()) != null)
            {
                lineNumber++;
                string line = raw.Trim();

                if (line.Length == 0 || line[0] == '#')
                    continue;

                if (line[0] == '[')
                {
                    if (string.Equals(line, "[PRIMARY]", StringComparison.OrdinalIgnoreCase))
                    {
                        isAlias = false;
                        sawSection = true;
                    }
                    else if (string.Equals(line, "[ALIAS]", StringComparison.OrdinalIgnoreCase))
                    {
                        isAlias = true;
                        sawSection = true;
                    }
                    else
                    {
                        errors.Add(new AlphabetError(lineNumber, "Unknown section '" + line + "'. Expected [PRIMARY] or [ALIAS]."));
                    }
                    continue;
                }

                if (!sawSection)
                {
                    errors.Add(new AlphabetError(lineNumber, "Entry appears before any [PRIMARY] or [ALIAS] section."));
                    continue;
                }

                int tab = raw.IndexOf('\t');
                if (tab <= 0)
                {
                    errors.Add(new AlphabetError(lineNumber, "Expected 'character<tab>pattern'."));
                    continue;
                }

                string charField = raw.Substring(0, tab);
                string pattern = raw.Substring(tab + 1).Trim();

                if (!TryDecodeChar(charField, out char character))
                {
                    errors.Add(new AlphabetError(lineNumber, "'" + charField + "' is not a single character or a \\uXXXX escape."));
                    continue;
                }

                try
                {
                    TablePacker.ParseCode(pattern);
                }
                catch (ArgumentException ex)
                {
                    errors.Add(new AlphabetError(lineNumber, ex.Message));
                    continue;
                }

                entries.Add(new MorseEntry(character, pattern, isAlias));
            }
        }

        if (entries.Count == 0 && errors.Count == 0)
            errors.Add(new AlphabetError(-1, "The file declares no characters."));

        return new ParsedAlphabet(name, filePath, entries, errors);
    }

    private static bool TryDecodeChar(string field, out char character)
    {
        if (field.Length == 1)
        {
            character = field[0];
            return true;
        }

        if (field.Length == 6 && (field[0] == '\\') && (field[1] == 'u' || field[1] == 'U') &&
            ushort.TryParse(field.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort value))
        {
            character = (char)value;
            return true;
        }

        character = '\0';
        return false;
    }
}
