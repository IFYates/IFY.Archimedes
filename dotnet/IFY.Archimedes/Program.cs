using IFY.Archimedes;
using IFY.Archimedes.ConsoleArgs;
using IFY.Archimedes.Logic;
using IFY.Archimedes.Models.Schema;
using System.Text.Json;
using YamlDotNet.Serialization;

// TODO: options
// - individual mermaid files
// - single MD file
// - interactive HTML
// - single complete mermaid
// - remove 'back' links
// - remove item links
// - add graph title
// - graph direction

var mermaidWriter = new MermaidWriter();

// Setup
string inFile = null!, outFile = null!;
var consoleOptions = new ConsoleOptions(
    new PositionalArg(0, "file", v => inFile = v, true, "Input file (JSON or YAML)."),
    new PositionalArg(1, "out", v => outFile = v, true, "Output file."),
    new NamedArg("dir", v => mermaidWriter.GraphDirection = v, false, "Graph direction (TD, LR). Default: LR")
);
if (!consoleOptions.TryParse(args))
{
    return;
}

if (mermaidWriter.GraphDirection is not "TD" and not "LR")
{
    Console.Error.WriteLine($"Invalid direction: {mermaidWriter.GraphDirection}. Must be TD or LR.");
    return;
}

// Read input
if (!File.Exists(inFile))
{
    Console.Error.WriteLine($"File not found: {inFile}");
    return;
}
var input = File.ReadAllText(inFile);

// Parse input
Dictionary<string, JsonEntry> source;
switch (new FileInfo(inFile).Extension.ToLower())
{
    case ".json":
    case ".jsonc":
        break;
    case ".yaml":
    case ".yml":
        var deserializer = new DeserializerBuilder().Build();
        var yaml = deserializer.Deserialize(new StringReader(input))
            ?? throw new InvalidDataException($"Failed to parse YAML: {inFile}");
        input = JsonSerializer.Serialize(yaml);
        break;
    default:
        throw new InvalidDataException($"Unsupported file type: {inFile}");
}
try
{
    source = JsonSerializer.Deserialize<Dictionary<string, JsonEntry>>(input, Utility.GlobalJsonOptions)
        ?? throw new InvalidDataException($"Failed to parse file: {inFile}");
}
catch (JsonException jex)
{
    throw new InvalidDataException($"Failed to parse file: {jex.Message}", jex);
}

// Validate schema
var validator = new SchemaValidator();
if (!validator.TryValidate(source))
{
    // TODO: output errors
    return;
}

// Build diagrams
var diagrams = DiagramBuilder.BuildDiagrams(validator.Result);

// Output
mermaidWriter.AllComponents = validator.Result;
var markdownWriter = new MarkdownWriter();

//// Output multiple mermaid files
//// TEMP
//for (var i = 0; i < diagrams.Count; i++)
//{
//    var d = diagrams.ToArray()[i].Value; // TEMP
//    var mermaid = MermaidWriter.WriteMermaid(d);
//    File.WriteAllText($@"F:\Dev\IFY.Archimedes\output-{i + 1}.mermaid", mermaid);
//}

// Output
var result = markdownWriter.Write(diagrams, mermaidWriter);
if (outFile is null)
{
    Console.WriteLine(result);
    return;
}

// TODO: writer defines out extension
outFile = Path.GetFullPath(outFile);
if (Directory.Exists(outFile))
{
    outFile = Path.Combine(outFile, "output.md");
}
File.WriteAllText(outFile, result);