using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace IFY.Archimedes.Models.Schema.Json;

/// <summary>
/// Represents an architecture component entry in the JSON data.
/// </summary>
public record JsonComponent(
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
    public Dictionary<string, JsonComponent> Children { get; init; } = [];
    /// <summary>
    /// Links to other components (by ID).
    /// Value can be the link type as a string, or a <see cref="JsonLink"/> object."/>
    /// </summary>
    public Dictionary<string, JsonElement> Links { get; init; } = [];

    /// <summary>
    /// Attempts to parse a JSON element into a <see cref="JsonComponent"/> instance using the specified key.
    /// </summary>
    /// <remarks>If parsing fails, detailed error information is provided in the <paramref name="errors"/>
    /// parameter. This method does not throw exceptions for invalid JSON input; instead, it returns <see
    /// langword="false"/> and supplies error details.</remarks>
    /// <param name="key">The unique identifier or name associated with the schema component being parsed. Used for error reporting.</param>
    /// <param name="json">The <see cref="JsonElement"/> containing the JSON representation of the schema component to parse.</param>
    /// <param name="component">When this method returns <see langword="true"/>, contains the parsed <see cref="JsonComponent"/> instance;
    /// otherwise, <see langword="null"/>.</param>
    /// <param name="errors">When this method returns <see langword="false"/>, contains an array of <see cref="SchemaError"/> objects
    /// describing the parsing errors; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the JSON element was successfully parsed into a <see cref="JsonComponent"/>;
    /// otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string key, JsonElement json, [NotNullWhen(true)] out JsonComponent? component, [NotNullWhen(false)] out SchemaError[]? errors)
    {
        try
        {
            component = json.Deserialize<JsonComponent>();
            if (component is null)
            {
                errors = [new(SchemaError.ErrorLevel.Error, $"Failed to parse schema component '{key}'.")];
                return false;
            }

            errors = null;
            return true;
        }
        catch (JsonException jex)
        {
            component = null;
            errors = [new SchemaError(SchemaError.ErrorLevel.Error, $"Failed to parse schema component '{key}': {jex.Message}", jex)];
            return false;
        }
    }
}
