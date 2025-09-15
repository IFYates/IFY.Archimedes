using IFY.Archimedes.Models.Schema;

namespace IFY.Archimedes.Models;

// TODO: support conflation in both directions
// TODO: support styling (generate ID)
public record NodeLink(string SourceId, string TargetId)
{
    public List<Link> Links { get; } = [];

    public LinkType Type => Links.Select(l => l.Style).Distinct().Count() == 1 ? Links.First().Style : LinkType.Default;
    public string? Text => Links.Select(l => l.Text ?? "").Distinct().Count() == 1 ? Links.First().Text : null;
}