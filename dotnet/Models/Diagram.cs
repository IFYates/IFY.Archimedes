using IFY.Archimedes.Models.Schema;

namespace IFY.Archimedes.Models;

/// <summary>
/// A single level diagram definition.
/// </summary>
public record Diagram
{
    /// <summary>
    /// The depth of the diagram from the root.
    /// </summary>
    public int Depth { get; set; }
    /// <summary>
    /// Gets the parent diagram of this diagram, if one exists.
    /// </summary>
    public Diagram? Parent { get; }
    /// <summary>
    /// The ID of the parent diagram, if any.
    /// </summary>
    public string? ParentId => Parent?.Id;
    /// <summary>
    /// The root component for the diagram, if any.
    /// </summary>
    public ArchComponent? RootComponent { get; }

    /// <summary>
    /// The unique ID of the diagram.
    /// </summary>
    public string Id { get; }
    /// <summary>
    /// The title of the diagram.
    /// </summary>
    public string Title { get; }

    /// <summary>
    /// Visible nodes.
    /// </summary>
    public Dictionary<string, DiagramNode> Nodes { get; } = [];
    /// <summary>
    ///  Visible links.
    /// </summary>
    public Dictionary<string, NodeLink> Links { get; } = [];

    public Diagram(ArchComponent? root, int depth, Diagram? parent)
    {
        Id = root is null ? "root-d" : $"d-{root.Id.ToLower()}";
        Title = root?.Title ?? "All Components";
        RootComponent = root;
        Depth = depth;
        Parent = parent;
    }

    public Dictionary<string, DiagramNode> GetAllNodes()
    {
        var all = new Dictionary<string, DiagramNode>();
        map(Nodes.Values);
        return all;
        void map(IEnumerable<DiagramNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (!all.ContainsKey(node.Id))
                {
                    all[node.Id] = node;
                    map(node.ChildNodes.Values);
                }
            }
        }
    }
}
