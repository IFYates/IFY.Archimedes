using IFY.Archimedes.Models;
using System.Text;

namespace IFY.Archimedes.Logic;

public class MarkdownWriter
{
    public string Write(Dictionary<string, Diagram> diagrams, MermaidWriter mermaidWriter)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# Architecture Diagrams");
        foreach (var diagram in diagrams.Values)
        {
            // TODO: options
            var mermaid = mermaidWriter.WriteMermaid(diagram);
            sb.AppendLine();
            sb.AppendLine($"<span id=\"{diagram.Id}\"></span>");
            sb.AppendLine($"## {diagram.Title}");
            // TODO: description, documents, link to parent
            sb.AppendLine();
            sb.AppendLine(":::mermaid");
            sb.AppendLine(mermaid);
            sb.AppendLine(":::");
        }
        return sb.ToString();
    }
}
