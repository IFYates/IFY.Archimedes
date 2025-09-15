using IFY.Archimedes.Logic;
using System.Text.Json.Serialization;

namespace IFY.Archimedes.Models.Schema.Json;

public record JsonNodeStyle(
    string? Base, // Missing = Default
    string? Color,
    string? Fill,
    [property: JsonPropertyName("font-size")]
    string? FontSize,
    string? Stroke,
    [property: JsonPropertyName("stroke-dasharray")]
    string? StrokeDashArray,
    [property: JsonPropertyName("stroke-width")]
    string? StrokeWidth
)
{
    public bool Validate(string name)
    {
        if (Base?.Length > 0
            && !NodeType.Types.ContainsKey(Base.ToLower()))
        {
            ErrorHandler.Error($"Node type '{name}' has invalid base type name '{Base}'.");
            return false;
        }

        // TODO: Validate properties

        return true;
    }

    public override string ToString()
    {
        return string.Join(", ", new[]
        {
            Color?.Length > 0 ? $"color: {Color}" : null,
            Fill?.Length > 0 ? $"fill: {Fill}" : null,
            FontSize?.Length > 0 ? $"font-size: {FontSize}" : null,
            Stroke?.Length > 0 ? $"stroke: {Stroke}" : null,
            StrokeDashArray?.Length > 0 ? $"stroke-dasharray: {StrokeDashArray}" : null,
            StrokeWidth?.Length > 0 ? $"stroke-width: {StrokeWidth}" : null,
        }.Where(s => s is not null));
    }
}
