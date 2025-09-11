namespace IFY.Archimedes.Models.Schema;


/// <summary>
/// A component in the architecture schema.
/// </summary>
public record ArchComponent(
    JsonEntry Source,
    ArchComponent? Parent,
    string Id,
    NodeType Type,
    string Title
)
{
    public string? Detail => Source.Detail;
    public string? Doc => Source.Doc;
    public bool Expand => Source.Expand;

    /// <summary>
    /// Direct children of this component.
    /// </summary>
    public Dictionary<string, ArchComponent> Children { get; } = [];

    /// <summary>
    /// Direct links from this component to other components (by ID).
    /// </summary>
    public Dictionary<string, Link> Links { get; } = [];
}
