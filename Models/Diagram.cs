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
    /// The ID of the parent diagram, if any.
    /// </summary>
    public string? ParentId { get; }

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

    public Diagram(ArchComponent? root, int depth)
    {
        Id = root is null ? "root-d" : $"d-{root.Id.ToLower()}";
        Title = root?.Title ?? "All Components";
        Depth = depth;
        ParentId = root is null ? null : root.Parent?.Id is null ? "root-d" : $"d-{root?.Parent?.Id.ToLower()}";
    }
}
