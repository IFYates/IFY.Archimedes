using IFY.Archimedes.Logic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace IFY.Archimedes.Models.Schema.Json;

/// <summary>
/// Represents an architecture component entry in the JSON data.
/// </summary>
public record JsonComponent(
    string? Style,
    string? Title,
    string? Detail,
    string? Doc,
    bool? Expand
)
{
    /// <summary>
    /// The child components of this entry.
    /// </summary>
    public IReadOnlyDictionary<string, JsonComponent> Children { get; init; } = new Dictionary<string, JsonComponent>();
    /// <summary>
    /// Links to other components (by ID).
    /// Value can be the link type as a string, or a <see cref="JsonLink"/> object."/>
    /// </summary>
    public IReadOnlyDictionary<string, JsonElement> Links { get; init; } = new Dictionary<string, JsonElement>();

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
    /// <returns><see langword="true"/> if the JSON element was successfully parsed into a <see cref="JsonComponent"/>;
    /// otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string key, JsonElement json, [NotNullWhen(true)] out JsonComponent? component)
    {
        try
        {
            component = json.Deserialize<JsonComponent>();
            if (component is null)
            {
                ErrorHandler.Error($"Failed to parse schema component '{key}'.");
                return false;
            }
            return true;
        }
        catch (JsonException jex)
        {
            component = null;
            ErrorHandler.Error($"Failed to parse schema component '{key}': {jex.Message}", jex);
            return false;
        }
    }

    /// <summary>
    /// Attempts to merge the specified JSON element into this <see cref="JsonComponent"/> instance.
    /// Throwing errors if the merge object attempts to overwrite existing values.
    /// </summary>
    /// <param name="key">The unique identifier or name associated with the schema component being parsed. Used for error reporting.</param>
    /// <param name="json">The <see cref="JsonElement"/> containing the JSON representation of the schema component to parse.</param>
    /// <param name="component">When this method returns <see langword="true"/>, contains the merged <see cref="JsonComponent"/> instance;
    /// otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if the JSON element was successfully merged into this <see cref="JsonComponent"/>;
    /// otherwise, <see langword="false"/>.</returns>
    public bool TryMerge(string key, JsonElement json, [NotNullWhen(true)] out JsonComponent? component)
    {
        try
        {
            if (!TryParse(key, json, out var mergeComponent) || mergeComponent is null)
            {
                component = null;
                return false;
            }

            // Check properties
            if (mergeComponent.Style is not null && Style is not null)
            {
                ErrorHandler.Error($"Cannot merge schema component '{key}': 'Style' property already exists.");
                component = null;
                return false;
            }
            if (mergeComponent.Title is not null && Title is not null)
            {
                ErrorHandler.Error($"Cannot merge schema component '{key}': 'Title' property already exists.");
                component = null;
                return false;
            }
            if (mergeComponent.Detail is not null && Detail is not null)
            {
                ErrorHandler.Error($"Cannot merge schema component '{key}': 'Detail' property already exists.");
                component = null;
                return false;
            }
            if (mergeComponent.Doc is not null && Doc is not null)
            {
                ErrorHandler.Error($"Cannot merge schema component '{key}': 'Doc' property already exists.");
                component = null;
                return false;
            }
            if (mergeComponent.Expand is not null && Expand is not null)
            {
                ErrorHandler.Error($"Cannot merge schema component '{key}': 'Expand' property already exists.");
                component = null;
                return false;
            }

            // Merge children
            var children = new Dictionary<string, JsonComponent>(Children);
            foreach (var child in mergeComponent.Children)
            {
                if (children.ContainsKey(child.Key))
                {
                    ErrorHandler.Error($"Cannot merge schema component: Child '{child.Key}' already exists.");
                    component = null;
                    return false;
                }
                children[child.Key] = child.Value;
            }

            // Merge links
            var links = new Dictionary<string, JsonElement>(Links);
            foreach (var link in mergeComponent.Links)
            {
                if (links.ContainsKey(link.Key))
                {
                    ErrorHandler.Error($"Cannot merge schema component: Link '{link.Key}' already exists.");
                    component = null;
                    return false;
                }
                links[link.Key] = link.Value;
            }

            // Create the merged component
            component = new JsonComponent(
                mergeComponent.Style ?? Style,
                mergeComponent.Title ?? Title,
                mergeComponent.Detail ?? Detail,
                mergeComponent.Doc ?? Doc,
                mergeComponent.Expand ?? Expand
            )
            {
                Children = children,
                Links = links
            };
            return true;
        }
        catch (JsonException jex)
        {
            ErrorHandler.Error($"Failed to parse schema component for merging: {jex.Message}", jex);
            component = null;
            return false;
        }
    }
}
