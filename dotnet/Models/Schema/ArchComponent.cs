using IFY.Archimedes.Models.Schema.Json;

namespace IFY.Archimedes.Models.Schema;


/// <summary>
/// A component in the architecture schema.
/// </summary>
public record ArchComponent(
    JsonComponent Source,
    ArchComponent? Parent,
    string Id,
    NodeType? Type,
    string Title
)
{
    public string? Detail => Source.Detail?.Replace("\\n", "\n");
    public string? Doc => Source.Doc;
    public bool Expand => Source.Expand;

    /// <summary>
    /// Direct children of this component.
    /// </summary>
    public Dictionary<string, ArchComponent> Children { get; } = [];

    /// <summary>
    /// Direct links from this component to other components (by ID).
    /// </summary>
    public List<Link> Links { get; } = [];
}
