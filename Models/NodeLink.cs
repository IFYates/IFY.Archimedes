using IFY.Archimedes.Models.Schema;

namespace IFY.Archimedes.Models;

public record NodeLink(string SourceId, string TargetId, Link Link)
{
    public LinkType Type { get; init; } = Link.Type;
    public string? Text { get; init; } = Link.Text;
}