using IFY.Archimedes.Models;
using IFY.Archimedes.Models.Schema;

namespace IFY.Archimedes.Logic;

/// <summary>
/// Resolves diagrams from schema components.
/// </summary>
public static class DiagramBuilder // TODO: options?
{
    public static Dictionary<string, Diagram> BuildDiagrams(Dictionary<string, ArchComponent> components)
    {
        var diagrams = new Dictionary<string, Diagram>();
        buildDiagram(diagrams, null, null, components);
        return diagrams;
    }

    // Root items are shown in detail with all links in and out
    // TODO: All root item parents should be shown
    private static void buildDiagram(Dictionary<string, Diagram> diagrams, ArchComponent? root, Diagram? parent, Dictionary<string, ArchComponent> components)
    {
        // Create diagram for the current root (if not already created)
        var depth = (parent?.Depth ?? -1) + 1;
        var diagram = new Diagram(root, depth, parent);
        if (diagrams.TryGetValue(diagram.Id, out var value))
        {
            if (value.Depth > depth)
            {
                value.Depth = depth;
            }
            return;
        }
        diagrams[diagram.Id] = diagram;

        var nodes = new Dictionary<string, DiagramNode>();
        var links = new List<Link>();

        if (root != null)
        {
            var node = addNode(null, root, true);

            // Add parent node hierarchy
            while (node.Component.Parent != null)
            {
                var parentNode = addNode(null, node.Component.Parent, false);
                parentNode.ChildNodes[node.Id] = node;
                diagram.Nodes.Remove(node.Id);
                diagram.Nodes[parentNode.Id] = parentNode;
                node = parentNode;
            }
        }
        else
        {
            // Add all top-level items
            foreach (var item in components.Values.Where(c => c.Parent is null))
            {
                addNode(null, item, item.Expand);
            }
        }

        // Find all other links to core diagram nodes
        var coreNodes = nodes.Keys.ToHashSet();
        foreach (var item in components.Values)
        {
            foreach (var link in item.Links)
            {
                var source = findVisibleItem(item.Id, out var sourceParent);
                var target = findVisibleItem(link.TargetId, out var targetParent);

                if (coreNodes.Contains(source.Id) || coreNodes.Contains(target.Id))
                {
                    if (!nodes.ContainsKey(source.Id))
                    {
                        addNode(sourceParent, source, source.Expand);
                    }
                    if (!nodes.ContainsKey(target.Id))
                    {
                        addNode(targetParent, target, target.Expand);
                    }
                }
            }
        }

        // Add links must be visible on the diagram
        foreach (var l in links)
        {
            var link = l;
            if (link.Reverse)
            {
                link = link with { SourceId = link.TargetId, TargetId = link.SourceId, Reverse = false };
            }

            var source = findVisibleItem(link.SourceId, out _);
            var target = findVisibleItem(link.TargetId, out _);

            if (source.Id == target.Id
                && link.SourceId != link.TargetId)
            {
                continue; // No implicit self-links
            }
            if (!coreNodes.Contains(source.Id) && !coreNodes.Contains(target.Id))
            {
                continue;
            }

            var linkId = $"{source.Id}:{target.Id}";
            if (!diagram.Links.TryGetValue(linkId, out var nodeLink))
            {
                nodeLink = new NodeLink(source.Id, target.Id);
                diagram.Links[linkId] = nodeLink;
            }

            nodeLink.Links.Add(link);
        }

        // Recursively build child diagrams
        foreach (var item in nodes.Values.Select(n => n.Component).Where(n => n.Children.Count > 0 && !n.Expand))
        {
            buildDiagram(diagrams, item, diagram, components);
        }

        DiagramNode addNode(DiagramNode? parent, ArchComponent component, bool expand)
        {
            // Register node
            var node = new DiagramNode(component);
            if (parent is null)
            {
                diagram.Nodes[node.Id] = node;
            }
            else
            {
                parent.ChildNodes[node.Id] = node;
            }
            nodes[node.Id] = node;

            mapLinks(component);

            // Expand children
            if (expand)
            {
                foreach (var child in component.Children.Values)
                {
                    addNode(node, child, child.Expand);
                }
            }

            return node;
        }
        ArchComponent findVisibleItem(string id, out DiagramNode? parent)
        {
            parent = null;
            var comp = components[id];
            while (comp.Parent != null
                && !nodes.ContainsKey(comp.Id))
            {
                if (nodes.TryGetValue(comp.Parent.Id, out var p)
                    && p.ChildNodes.Count > 0)
                {
                    parent = p;
                    break;
                }

                comp = comp.Parent;
            }
            return comp;
        }
        void mapLinks(ArchComponent component)
        {
            links.AddRange(component.Links);
            foreach (var child in component.Children.Values)
            {
                mapLinks(child);
            }
        }
    }
}
