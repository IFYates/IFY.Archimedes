using IFY.Archimedes.Logic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace IFY.Archimedes.Models.Schema.Json;

/// <summary>
/// Represents the configuration settings for a diagram, as defined in a JSON schema.
/// </summary>
public class JsonConfig
{
    /// <summary>
    /// Gets the default configuration for JSON-based architecture diagrams.
    /// </summary>
    /// <remarks>Use this instance as a baseline when creating new configurations to ensure consistent default
    /// settings for diagram direction and title.</remarks>
    public static readonly JsonConfig Default = new()
    {
        Direction = "TD",
        Title = "Architecture Diagram",
    };

    public string Direction { get; set; } = "TD";
    public string? Title { get; set; }
    public Dictionary<string, JsonNodeStyle> NodeTypes { get; set; } = [];
    public Dictionary<string, JsonElement> LinkTypes { get; set; } = [];

    /// <summary>
    /// Attempts to parse the specified JSON element into a <see cref="JsonConfig"> instance.
    /// </summary>
    /// <param name="json">The JSON element containing the configuration data to parse.</param>
    /// <param name="config">When this method returns, contains the parsed <see cref="JsonConfig"> instance if parsing succeeds; otherwise, <see langword="null"/>. This
    /// parameter is passed uninitialized.</param>
    /// <returns><see langword="true"/> if the JSON element was successfully parsed into a <see cref="JsonConfig"> instance; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(JsonElement json, [NotNullWhen(true)] out JsonConfig? config)
    {
        try
        {
            config = json.Deserialize<JsonConfig>() ?? new();

            // Validate
            if (config.Direction is not "TD" and not "LR")
            {
                ErrorHandler.Error($"Invalid direction: {config.Direction}. Must be TD or LR.");
                return false;
            }

            var failed = !config.NodeTypes.Select(t => t.Value.Validate(t.Key)).All(v => v);
            foreach (var type in config.NodeTypes)
            {
                if (NodeType.Types.ContainsKey(type.Key.ToLower()))
                {
                    ErrorHandler.Error($"Duplicate node type: {type.Key}");
                    return false;
                }

                var baseTypeName = type.Value.Base?.Length > 0 ? type.Value.Base : "default";
                if (!NodeType.Types.TryGetValue(baseTypeName.ToLower(), out var baseType))
                {
                    baseType = NodeType.Default;
                }

                NodeType.Types[type.Key.ToLower()] = baseType with
                {
                    Style = type.Value
                };
            }

            return !failed;
        }
        catch (JsonException jex)
        {
            config = null;
            ErrorHandler.Error($"Failed to parse schema config: {jex.Message}", jex);
            return false;
        }
    }
}
