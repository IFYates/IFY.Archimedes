using IFY.Archimedes.Models;
using IFY.Archimedes.Models.Schema;
using System.Text;

namespace IFY.Archimedes.Logic;

public static class MermaidWriter
{
    public static string WriteMermaid(Diagram diagram, Dictionary<string, ArchComponent> all)
    {
        var sb = new StringBuilder();
        sb.AppendLine("graph TD");
        sb.AppendLine($"%% {diagram.Title}");
        sb.AppendLine();

        // Write nodes
        foreach (var node in diagram.Nodes.Values)
        {
            writeNode(sb, node, 1);
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
                    var target = all[info.TargetId];
                    if (target.Parent != null)
                    {
                        lines.Add($"<small>To <a href='#d-{target.Parent.Id.ToLower()}'>{target.Title.HtmlEncode()}</a></small>");
                    }
                }
                else if (!nodes.ContainsKey(info.SourceId) && nodes.ContainsKey(info.TargetId))
                {
                    var source = all[info.SourceId];
                    if (source.Parent != null)
                    {
                        lines.Add($"<small>From <a href='#d-{source.Parent.Id.ToLower()}'>{source.Title.HtmlEncode()}</a></small>");
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

        if (diagram.ParentId != null)
        {
            sb.AppendLine();
            sb.AppendLine($"B{DateTime.Now.Ticks}([\"<small><a href='#{diagram.ParentId}'>Back</a></small>\"])");
        }

        return sb.ToString();
    }

    private static void writeNode(StringBuilder sb, DiagramNode node, int depth)
    {
        var indent = new string(' ', depth * 4);

        var isSubgraph = node.ChildNodes.Count > 0;
        var childCount = node.Component.Children.Count;

        var nodeLabel = !isSubgraph && childCount > 0
            ? $"<a href='#d-{node.Id.ToLower()}' title='Expand node'>{node.Title.HtmlEncode()}</a>"
            : node.Title.HtmlEncode();

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
                writeNode(sb, child, depth + 1);
            }
            sb.AppendLine($"{indent}end");
        }
        else
        {
            if (node.Component.Detail != null)
            {
                nodeLabel += $"<br>{node.Component.Detail.HtmlEncode()}";
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

            var nodeType = node.Type;
            if (nodeType == NodeType.Default)
            {
                nodeType = node.Component.Children.Count > 0 ? NodeType.Block : NodeType.Node;
            }
            var shape = nodeType.GetEnumMemberValue();
            sb.AppendLine($"{indent}{node.Id}{string.Format(shape, nodeLabel)}");
        }
    }
}
