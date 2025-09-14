using IFY.Archimedes.Logic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace IFY.Archimedes.Models.Schema.Json;

/// <summary>
/// Represents the configuration settings for a diagram, as defined in a JSON schema.
/// </summary>
public class JsonConfig
{
    public string Direction { get; set; } = "TD";
    public string? Title { get; set; }
    public Dictionary<string, JsonElement> NodeTypes { get; } = [];
    public Dictionary<string, JsonElement> LinkTypes { get; } = [];

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
            return true;
        }
        catch (JsonException jex)
        {
            config = null;
            ErrorHandler.Error($"Failed to parse schema config: {jex.Message}", jex);
            return false;
        }
    }
}
