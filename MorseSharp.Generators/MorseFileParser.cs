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
/// [PROSIGN]
/// SK&#9;...-.-
/// [PROSIGN ALIAS]
/// AR&#9;.-.-.
/// </code>
/// One entry per line, the name and its pattern separated by a tab. A character that would confuse the parser can be
/// written as <c>\uXXXX</c>. The alias sections hold entries whose pattern something else already owns.
/// </remarks>
internal static class MorseFileParser
{
    private enum Section
    {
        None,
        Primary,
        Alias,
        Prosign,
        ProsignAlias,
    }

    /// <summary>Parses the text of one file.</summary>
    /// <param name="name">The alphabet name, taken from the file name.</param>
    /// <param name="filePath">Path of the file, for diagnostics.</param>
    /// <param name="text">The file contents.</param>
    public static ParsedAlphabet Parse(string name, string filePath, string text)
    {
        List<MorseEntry> entries = new List<MorseEntry>();
        List<MorseProsign> prosigns = new List<MorseProsign>();
        List<AlphabetError> errors = new List<AlphabetError>();

        Section section = Section.None;
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
                    section = ReadSection(line);
                    if (section == Section.None)
                        errors.Add(new AlphabetError(lineNumber, "Unknown section '" + line + "'. Expected [PRIMARY], [ALIAS], [PROSIGN] or [PROSIGN ALIAS]."));
                    continue;
                }

                if (section == Section.None)
                {
                    errors.Add(new AlphabetError(lineNumber, "Entry appears before any section header."));
                    continue;
                }

                int tab = raw.IndexOf('\t');
                if (tab <= 0)
                {
                    errors.Add(new AlphabetError(lineNumber, "Expected 'name<tab>pattern'."));
                    continue;
                }

                string nameField = raw.Substring(0, tab).Trim();
                string pattern = raw.Substring(tab + 1).Trim();

                try
                {
                    TablePacker.ParseCode(pattern);
                }
                catch (ArgumentException ex)
                {
                    errors.Add(new AlphabetError(lineNumber, ex.Message));
                    continue;
                }

                if (section == Section.Prosign || section == Section.ProsignAlias)
                {
                    if (nameField.Length < 2)
                    {
                        errors.Add(new AlphabetError(lineNumber, "A prosign needs at least two letters; '" + nameField + "' has one."));
                        continue;
                    }

                    prosigns.Add(new MorseProsign(nameField, pattern, section == Section.ProsignAlias));
                    continue;
                }

                if (!TryDecodeChar(raw.Substring(0, tab), out char character))
                {
                    errors.Add(new AlphabetError(lineNumber, "'" + nameField + "' is not a single character or a \\uXXXX escape."));
                    continue;
                }

                entries.Add(new MorseEntry(character, pattern, section == Section.Alias));
            }
        }

        if (entries.Count == 0 && errors.Count == 0)
            errors.Add(new AlphabetError(-1, "The file declares no characters."));

        return new ParsedAlphabet(name, filePath, entries, prosigns, errors);
    }

    private static Section ReadSection(string line)
    {
        if (string.Equals(line, "[PRIMARY]", StringComparison.OrdinalIgnoreCase))
            return Section.Primary;
        if (string.Equals(line, "[ALIAS]", StringComparison.OrdinalIgnoreCase))
            return Section.Alias;
        if (string.Equals(line, "[PROSIGN]", StringComparison.OrdinalIgnoreCase))
            return Section.Prosign;
        if (string.Equals(line, "[PROSIGN ALIAS]", StringComparison.OrdinalIgnoreCase))
            return Section.ProsignAlias;
        return Section.None;
    }

    private static bool TryDecodeChar(string field, out char character)
    {
        if (field.Length == 1)
        {
            character = field[0];
            return true;
        }

        if (field.Length == 6 && field[0] == '\\' && (field[1] == 'u' || field[1] == 'U') &&
            ushort.TryParse(field.Substring(2), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort value))
        {
            character = (char)value;
            return true;
        }

        character = '\0';
        return false;
    }
}
