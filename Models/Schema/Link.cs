namespace IFY.Archimedes.Models.Schema;

/// <summary>
/// A link between two <see cref="ArchComponent"/>s.
/// </summary>
public record Link(LinkType Type, string? Text);
