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
        buildDiagram(diagrams, null, 0, components);
        return diagrams;
    }

    // Root items are shown in detail with all links in and out
    // TODO: All root item parents should be shown
    private static void buildDiagram(Dictionary<string, Diagram> diagrams, ArchComponent? root, int depth, Dictionary<string, ArchComponent> components)
    {
        // Create diagram for the current root (if not already created)
        var diagram = new Diagram(root, depth);
        if (diagrams.TryGetValue(diagram.Id, out var value))
        {
            if (value.Depth > depth)
            {
                value.Depth = depth;
            }
            return;
        }
        diagrams[diagram.Id] = diagram;

        var nodes = new Dictionary<string, ArchComponent>();
        var links = new List<Link>();

        if (root != null)
        {
            addNode(null, root, true);
        }
        else
        {
            // Add all top-level items
            foreach (var item in components.Values.Where(c => c.Parent is null))
            {
                addNode(null, item, false);
            }
        }

        // Find all other links to core diagram nodes
        var coreNodes = nodes.Keys.ToHashSet();
        foreach (var item in components.Values)
        {
            foreach (var link in item.Links)
            {
                var source = findVisibleItem(item.Id);
                var target = findVisibleItem(link.TargetId);

                if (coreNodes.Contains(source.Id) || coreNodes.Contains(target.Id))
                {
                    if (!nodes.ContainsKey(source.Id))
                    {
                        addNode(null, source, false);
                    }
                    if (!nodes.ContainsKey(target.Id))
                    {
                        addNode(null, target, false);
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

            var source = findVisibleItem(link.SourceId);
            var target = findVisibleItem(link.TargetId);

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
        foreach (var item in nodes.Values.Where(n => n.Children.Count > 0 && !n.Expand))
        {
            buildDiagram(diagrams, item, depth + 1, components);
        }

        void addNode(DiagramNode? parent, ArchComponent component, bool expand)
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
            nodes[component.Id] = component;

            mapLinks(component);

            // Expand children
            if (expand || component.Expand)
            {
                foreach (var child in component.Children.Values)
                {
                    addNode(node, child, false);
                }
            }
        }
        ArchComponent findVisibleItem(string id)
        {
            var comp = components[id];
            while (comp.Parent != null && !nodes.ContainsKey(comp.Id))
            {
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
