using IFY.Archimedes;
using IFY.Archimedes.Logic;

// TODO: options
//
// General options:
// * -file <input file>
// * -out <output file or directory> (default stdout)
// * -format MD | MARKDOWN | MM | MERMAID | HTML (mermaid options required for all)
//
// MD/MARKDOWN options:
// * -mm-tag ':'/'`' (for mermaid blocks)
//
// MM/MERMAID options:
// * -whole (single diagram with all items)
// * -no-back (no back links)
// * -no-link (don't show item links)
// * -no-doc (no doc links)

// Setup
var options = new ConsoleArgs(args).Parse().ToArray();

// Read input
var inFiles = options.Where(o => o.Name == "file" && o.Value?.Length > 0).Select(a => a.Value!).ToArray();
if (inFiles.Length == 0)
{
    inFiles = [options.SingleOrDefault(o => o.Position == 0)?.Value!];
}
if (inFiles.Length == 0 || inFiles[0] is null)
{
    ErrorHandler.Error("No input file specified.");
    return;
}
var validator = new SchemaValidator();
foreach (var inFile in inFiles)
{
    validator.AddFile(inFile);
}

// Validate schema
if (!validator.Validate())
{
    return;
}

// Build diagrams
var diagrams = DiagramBuilder.BuildDiagrams(validator.Result);

// Output
var mermaidWriter = new MermaidWriter(validator.Result, validator.Config);
var markdownWriter = new MarkdownWriter(validator.Config);

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