using IFY.Archimedes;
using IFY.Archimedes.Logic;
using IFY.Archimedes.Models.Schema;
using System.Text.Json;
using YamlDotNet.Serialization;

// TODO: options
//
// General options:
// * -file <input file>
// * -out <output file or directory> (default stdout)
// * -format MD | MARKDOWN | MM | MERMAID | HTML (mermaid options required for all)
// * 
//
// MD/MARKDOWN options:
// * -mm-tag ':'/'`' (for mermaid blocks)
// * -title <title> (default from root item or "Architecture Diagram")
//
// MM/MERMAID options:
// * -dir TD/LR
// * -whole (single diagram with all items)
// * -no-back (no back links)
// * -no-link (don't show item links)
// * -no-doc (no doc links)

var mermaidWriter = new MermaidWriter();

// Setup
var options = new ConsoleArgs(args).Parse().ToArray();

mermaidWriter.GraphDirection = options.SingleOrDefault(o => o.Name == "dir")?.Value ?? "LR";
if (mermaidWriter.GraphDirection is not "TD" and not "LR")
{
    Console.Error.WriteLine($"Invalid direction: {mermaidWriter.GraphDirection}. Must be TD or LR.");
    return;
}

// Read input
var inFile = options.SingleOrDefault(o => o.Position == 0 || o.Name == "file")?.Value;
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

var outFile = options.SingleOrDefault(o => o.Position == 1 || o.Name == "out")?.Value;
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