using IFY.Archimedes;
using IFY.Archimedes.Logic;

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
    ErrorHandler.Error($"Invalid direction: {mermaidWriter.GraphDirection}. Must be TD or LR.");
    return;
}

// Read input
var inFiles = options.Where(o => o.Name == "file" && o.Value?.Length > 0).Select(a => a.Value!).ToArray();
if (inFiles.Length == 0)
{
    inFiles = [options.SingleOrDefault(o => o.Position == 0)?.Value!];
}
if (inFiles.Length == 0 || inFiles[0]?.Length > 0)
{
    ErrorHandler.Error("No input file specified.");
    return;
}
var validator = new SchemaValidator();
foreach (var inFile in inFiles)
{
    validator.AddSchema(inFile);
}

// Validate schema
if (!validator.Validate())
{
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