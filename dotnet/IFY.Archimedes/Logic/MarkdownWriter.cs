using IFY.Archimedes.Models;
using IFY.Archimedes.Models.Schema.Json;
using System.Text;

namespace IFY.Archimedes.Logic;

public class MarkdownWriter(JsonConfig config)
{
    private readonly JsonConfig _config = config;

    public string Write(Dictionary<string, Diagram> diagrams, MermaidWriter mermaidWriter)
    {
        var sb = new StringBuilder();
        sb.AppendLine("# " + _config.Title);
        foreach (var diagram in diagrams.Values)
        {
            // TODO: options
            var mermaid = mermaidWriter.WriteMermaid(diagram);
            sb.AppendLine();
            sb.AppendLine($"<span id=\"{diagram.Id}\"></span>");

            var title = diagram.Title;
            var parent = diagram.Parent;
            while (parent?.Parent != null)
            {
                title = $"<a href='#{parent.Id}'>{parent.Title}</a> / {title}";
                parent = parent.Parent;
            }
            sb.AppendLine($"## {title}");
            // TODO: description, documents, link to parent
            sb.AppendLine();
            sb.AppendLine(":::mermaid");
            sb.AppendLine(mermaid);
            sb.AppendLine(":::");
        }
        return sb.ToString();
    }
}
