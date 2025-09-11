using IFY.Archimedes.Models.Schema;

namespace IFY.Archimedes.Models;

public record NodeLink(string SourceId, string TargetId)
{
    public List<Link> Links { get; } = [];

    public LinkType Type => Links.Select(l => l.Type).Distinct().Count() == 1 ? Links.First().Type : LinkType.Default;
    public string? Text => Links.Select(l => l.Text ?? "").Distinct().Count() == 1 ? Links.First().Text : null;
}