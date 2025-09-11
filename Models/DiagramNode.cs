using IFY.Archimedes.Models.Schema;

namespace IFY.Archimedes.Models;

/// <summary>
/// A use of a component in a diagram, with diagram-specific data.
/// </summary>
public record DiagramNode(ArchComponent Component)
{
    public string Id => Component.Id;
    public string Title => Component.Title;
    public NodeType Type => Component.Type;
    public Dictionary<string, DiagramNode> ChildNodes { get; } = [];
}