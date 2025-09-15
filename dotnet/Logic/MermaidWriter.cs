using IFY.Archimedes.Models;
using IFY.Archimedes.Models.Schema;
using IFY.Archimedes.Models.Schema.Json;
using System.Text;

namespace IFY.Archimedes.Logic;

public class MermaidWriter(
    Dictionary<string, ArchComponent> allComponents,
    JsonConfig config)
{
    public string WriteMermaid(Diagram diagram)
    {
        var sb = new StringBuilder();
        sb.AppendLine("graph " + config.Direction);
        sb.AppendLine($"%% {diagram.Title}");

        if (diagram.ParentId != null)
        {
            sb.AppendLine($"    B{DateTime.Now.Ticks}([\"<small><a href='#{diagram.ParentId}'>Back</a></small>\"])");
            sb.AppendLine();
        }

        // Write nodes
        foreach (var node in diagram.Nodes.Values)
        {
            writeNode(sb, node, 1, diagram);
        }
        sb.AppendLine();

        // Write links
        var nodes = diagram.GetAllNodes();
        foreach (var link in diagram.Links.Values)
        {
            var arrow = link.Type.GetEnumMemberValue();
            List<string?> lines = [link.Text?.HtmlEncode()];

            // If showing one true end node and inherited other, link to true node
            foreach (var info in link.Links)
            {
                if (nodes.ContainsKey(info.SourceId) && !nodes.ContainsKey(info.TargetId))
                {
                    var target = allComponents[info.TargetId];
                    if (target.Parent != null)
                    {
                        lines.Add($"<small><em>To <a href='#d-{target.Parent.Id.ToLower()}'>{target.Title.HtmlEncode()}</a></em></small>");
                    }
                }
                else if (!nodes.ContainsKey(info.SourceId) && nodes.ContainsKey(info.TargetId))
                {
                    var source = allComponents[info.SourceId];
                    if (source.Parent != null)
                    {
                        lines.Add($"<small><em>From <a href='#d-{source.Parent.Id.ToLower()}'>{source.Title.HtmlEncode()}</a></em></small>");
                    }
                }
            }

            var text = string.Join("<br>", lines.Where(l => !string.IsNullOrWhiteSpace(l)).Distinct());
            if (text?.Length > 0)
            {
                arrow += $"|\"{text}\"|";
            }

            sb.AppendLine($"    {link.SourceId} {arrow} {link.TargetId}");
        }

        return sb.ToString().TrimEnd();
    }

    private static void writeNode(StringBuilder sb, DiagramNode node, int depth, Diagram diagram)
    {
        var indent = new string(' ', depth * 4);

        var isSubgraph = node.ChildNodes.Count > 0;
        var childCount = node.Component.Children.Count;

        var nodeLabel = node.Title.HtmlEncode();
        if (!isSubgraph && childCount > 0)
        {
            nodeLabel = $"<a href='#d-{node.Id.ToLower()}' title='Expand node'>{nodeLabel}</a>";
        }
        if (node.Id == diagram.RootComponent?.Id)
        {
            nodeLabel = $"<big><strong>{nodeLabel}</strong></big>";
        }

        if (isSubgraph)
        {
            if (node.Component.Detail != null)
            {
                nodeLabel += $" <sup><span title='{node.Component.Detail}'>ℹ️</span></sup>";
            }
            if (node.Component.Doc != null)
            {
                nodeLabel += $" <sup><a href='{node.Component.Doc}' title='Go to documentation'>📖</a></sup>";
            }

            sb.AppendLine($"{indent}subgraph {node.Id}[\"{nodeLabel}\"]");
            foreach (var child in node.ChildNodes.Values)
            {
                writeNode(sb, child, depth + 1, diagram);
            }
            sb.AppendLine($"{indent}end");
        }
        else
        {
            if (node.Component.Detail != null)
            {
                nodeLabel += $"<br><em>{node.Component.Detail.HtmlEncode()}</em>";
            }
            if (node.Component.Doc != null)
            {
                nodeLabel += $"<br><small><a href='{node.Component.Doc}' title='Go to documentation'>📖 Documentation</a></small>";
            }
            switch (childCount)
            {
                case 1:
                    nodeLabel += $"<br><small>1 child</small>";
                    break;
                case > 0:
                    nodeLabel += $"<br><small>{childCount} children</small>";
                    break;
            }

            var nodeFormat = node.Type?.Format;
            nodeFormat ??= node.Component.Children.Count > 0
                    ? NodeType.Block.Format : NodeType.Node.Format;
            sb.AppendLine($"{indent}{node.Id}{string.Format(nodeFormat, nodeLabel)}");

            var style = node.Type?.Style?.ToString();
            if (style?.Length > 0)
            {
                sb.AppendLine($"{indent}style {node.Id} {style}");
            }
        }
    }
}
