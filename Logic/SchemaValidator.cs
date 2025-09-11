using IFY.Archimedes.Models.Schema;
using System.Text.Json;

namespace IFY.Archimedes.Logic;

public partial class SchemaValidator
{
    public List<string> Errors { get; } = [];

    public Dictionary<string, ArchComponent>? Result { get; private set; }

    /// <summary>
    /// Returns flattened structure of components with validated properties.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(Result))]
    public bool TryValidate(Dictionary<string, JsonEntry> schema)
    {
        var all = new Dictionary<string, ArchComponent>();

        // Ensure all components are unique and valid
        foreach (var key in schema.Keys)
        {
            parseComponents(key, schema[key]);
        }

        // Validate and resolve all links
        foreach (var comp in all.Values)
        {
            foreach (var link in comp.Source.Links)
            {
                // Can be string or JsonLink
                string? typeValue = null;
                string? linkText = null;
                if (link.Value.ValueKind == JsonValueKind.String)
                {
                    typeValue = link.Value.GetString();
                }
                else if (link.Value.ValueKind == JsonValueKind.Object)
                {
                    var obj = link.Value.Deserialize<JsonLink>(Utility.GlobalJsonOptions);
                    if (obj is null)
                    {
                        Errors.Add($"Unable to deserialise link object for component '{comp.Id}' and link '{link.Key}'.");
                    }
                    else
                    {
                        typeValue = obj.Type;
                        linkText = obj.Text;
                    }
                }
                else
                {
                    Errors.Add($"Unable to deserialise link object for component '{comp.Id}' and link '{link.Key}'.");
                }

                if (!Enum.TryParse(typeValue ?? "default", true, out LinkType linkType))
                {
                    throw new InvalidDataException($"Component '{comp.Id}' has an invalid link type '{link.Value}' for target '{link.Key}'.");
                }
                if (!all.ContainsKey(link.Key))
                {
                    throw new InvalidDataException($"Component '{comp.Id}' has a link to undefined target '{link.Key}'.");
                }
                comp.Links.Add(new(comp.Id, link.Key, linkType, linkText));
            }
        }

        Result = all;
        return true;

        ArchComponent parseComponents(string key, JsonEntry item, ArchComponent? parent = null)
        {
            // Validate key format: ^\w+$
            if (!ComponentIdFormat().IsMatch(key))
            {
                throw new InvalidDataException($"Component key '{key}' is invalid. Keys must be non-empty and contain only letters, numbers, and underscores.");
            }

            if (all.ContainsKey(key))
            {
                throw new InvalidDataException($"Component '{key}' is defined multiple times.");
            }

            var nodeType = NodeType.Default;
            if (item.Type?.Length > 0 && !Enum.TryParse(item.Type, true, out nodeType))
            {
                throw new InvalidDataException($"Component '{key}' has an invalid 'Type' property: {item.Type}");
            }

            var comp = new ArchComponent(item, parent, key, nodeType, item.Title?.Length > 0 ? item.Title : key);
            all[key] = comp;

            foreach (var childKey in item.Children.Keys)
            {
                comp.Children[childKey] = parseComponents(childKey, item.Children[childKey], comp);
            }

            return comp;
        }
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^\w+$")]
    private static partial System.Text.RegularExpressions.Regex ComponentIdFormat();
}
