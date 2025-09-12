using System.Text.Json;

namespace IFY.Archimedes.Models.Schema;

/// <summary>
/// Represents an architecture component entry in the JSON data.
/// </summary>
public record JsonEntry(
    string? Type,
    string? Title,
    string? Detail,
    string? Doc,
    string? Style,
    bool Expand
)
{
    /// <summary>
    /// The child components of this entry.
    /// </summary>
    public Dictionary<string, JsonEntry> Children { get; init; } = [];
    /// <summary>
    /// Links to other components (by ID).
    /// Value can be the link type as a string, or a <see cref="JsonLink"/> object."/>
    /// </summary>
    public Dictionary<string, JsonElement> Links { get; init; } = [];
}
