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

// Setup
string file = null!;
var consoleOptions = new ConsoleOptions(
    new PositionalArg(0, "file", v => file = v, true, "Input file (JSON or YAML)."),
    new NamedArg("dir", v => Options.Direction = v, false, "Graph direction (TD, LR). Default: LR")
);
if (!consoleOptions.TryParse(args))
{
    return;
}

if (Options.Direction is not "TD" and not "LR")
{
    Console.Error.WriteLine($"Invalid direction: {Options.Direction}. Must be TD or LR.");
    return;
}

// Read input
if (!File.Exists(file))
{
    Console.Error.WriteLine($"File not found: {file}");
    return;
}
var input = File.ReadAllText(file);

// Parse input
Dictionary<string, JsonEntry> source;
switch (new FileInfo(file).Extension.ToLower())
{
    case ".json":
    case ".jsonc":
        break;
    case ".yaml":
    case ".yml":
        var deserializer = new DeserializerBuilder().Build();
        var yaml = deserializer.Deserialize(new StringReader(input))
            ?? throw new InvalidDataException($"Failed to parse YAML: {file}");
        input = JsonSerializer.Serialize(yaml);
        break;
    default:
        throw new InvalidDataException($"Unsupported file type: {file}");
}
try
{
    source = JsonSerializer.Deserialize<Dictionary<string, JsonEntry>>(input, Utility.GlobalJsonOptions)
        ?? throw new InvalidDataException($"Failed to parse file: {file}");
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

//// Output multiple mermaid files
//// TEMP
//for (var i = 0; i < diagrams.Count; i++)
//{
//    var d = diagrams.ToArray()[i].Value; // TEMP
//    var mermaid = MermaidWriter.WriteMermaid(d);
//    File.WriteAllText($@"F:\Dev\IFY.Archimedes\output-{i + 1}.mermaid", mermaid);
//}

// Output single MD file
// TODO: To writer
var sb = new System.Text.StringBuilder();
sb.AppendLine("# Architecture Diagrams");
foreach (var diagram in diagrams.Values)
{
    // TODO: options
    var mermaid = MermaidWriter.WriteMermaid(diagram, validator.Result);
    sb.AppendLine();
    sb.AppendLine($"<span id=\"{diagram.Id}\"></span>");
    sb.AppendLine($"## {diagram.Title}");
    // TODO: description, documents, link to parent
    sb.AppendLine();
    sb.AppendLine(":::mermaid");
    sb.AppendLine(mermaid);
    sb.AppendLine(":::");
}
File.WriteAllText($@"F:\Dev\IFY.Archimedes\output.md", sb.ToString());

static class Options
{
    public static string Direction { get; set; } = "LR";
}