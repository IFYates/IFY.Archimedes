using IFY.Archimedes;
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

// Read input
var file = "../../../arch.jsonc";
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
var sb = new System.Text.StringBuilder();
sb.AppendLine("# Architecture Diagrams");
foreach (var diagram in diagrams.Values)
{
    var mermaid = MermaidWriter.WriteMermaid(diagram);
    sb.AppendLine();
    sb.AppendLine($"<span id=\"{diagram.Id}\"></span>");
    sb.AppendLine($"## {diagram.Title}");
    sb.AppendLine();
    sb.AppendLine(":::mermaid");
    sb.AppendLine(mermaid);
    sb.AppendLine(":::");
}
File.WriteAllText($@"F:\Dev\IFY.Archimedes\output.md", sb.ToString());