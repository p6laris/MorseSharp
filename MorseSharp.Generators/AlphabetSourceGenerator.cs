using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using MorseSharp.Alphabet;

namespace MorseSharp.Generators;

/// <summary>
/// Packs every <c>.morse</c> file into lookup tables at build time and emits them as byte blobs.
/// </summary>
/// <remarks>
/// <para>
/// This runs the very same <see cref="TablePacker"/> the library uses at run time, compiled into this assembly from
/// a linked source file. That is the point of the whole exercise: the tables shipped in the assembly are not a
/// reimplementation of the runtime ones, they are the output of the identical algorithm, so the two cannot drift.
/// </para>
/// <para>
/// A broken table also stops being a run-time surprise. A duplicate pattern or a bad symbol becomes a compiler error
/// against the offending line of the data file, rather than an exception thrown the first time someone selects that
/// language in production.
/// </para>
/// </remarks>
[Generator(LanguageNames.CSharp)]
public sealed class AlphabetSourceGenerator : IIncrementalGenerator
{
    private const string FileExtension = ".morse";

    private static readonly DiagnosticDescriptor InvalidAlphabet = new(
        id: "MORSE001",
        title: "Invalid Morse alphabet",
        messageFormat: "{0}",
        category: "MorseSharp",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValuesProvider<ParsedAlphabet> alphabets = context.AdditionalTextsProvider
            .Where(static file => file.Path.EndsWith(FileExtension, StringComparison.OrdinalIgnoreCase))
            .Select(static (file, cancellationToken) =>
            {
                string name = Path.GetFileNameWithoutExtension(file.Path);
                SourceText? text = file.GetText(cancellationToken);
                return text is null
                    ? new ParsedAlphabet(name, file.Path, new List<MorseEntry>(), new List<MorseProsign>(), [new AlphabetError(-1, "The file could not be read.")])
                    : MorseFileParser.Parse(name, file.Path, text.ToString());
            });

        context.RegisterSourceOutput(alphabets, static (context, alphabet) => EmitAlphabet(context, alphabet));
        context.RegisterSourceOutput(alphabets.Collect(), static (context, all) => EmitRegistry(context, all));
    }

    private static void EmitAlphabet(SourceProductionContext context, ParsedAlphabet alphabet)
    {
        if (Report(context, alphabet, alphabet.Errors))
            return;

        PackedTables tables;
        try
        {
            tables = TablePacker.Pack(alphabet.Name, alphabet.Entries, alphabet.Prosigns);
        }
        catch (Exception ex) when (ex is InvalidOperationException or ArgumentException)
        {
            Report(context, alphabet, [new AlphabetError(-1, ex.Message)], prefixName: false);
            return;
        }

        context.AddSource(alphabet.Name + "Characters.g.cs", AlphabetEmitter.EmitCharacters(alphabet, tables));
    }

    private static void EmitRegistry(SourceProductionContext context, ImmutableArray<ParsedAlphabet> alphabets)
    {
        List<string> names = alphabets
            .Where(static alphabet => alphabet.Errors.Count == 0)
            .Select(static alphabet => alphabet.Name)
            .OrderBy(static name => name, StringComparer.Ordinal)
            .ToList();

        if (names.Count == 0)
            return;

        context.AddSource("Alphabets.g.cs", AlphabetEmitter.EmitRegistry(names));
    }

    private static bool Report(SourceProductionContext context, ParsedAlphabet alphabet, List<AlphabetError> errors, bool prefixName = true)
    {
        foreach (AlphabetError error in errors)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                InvalidAlphabet,
                CreateLocation(alphabet.FilePath, error.Line),
                prefixName ? alphabet.Name + ": " + error.Message : error.Message));
        }

        return errors.Count > 0;
    }

    private static Location CreateLocation(string path, int line)
    {
        if (line < 0)
            return Location.None;

        LinePositionSpan span = new(new LinePosition(line, 0), new LinePosition(line, 0));
        return Location.Create(path, new TextSpan(0, 0), span);
    }
}
