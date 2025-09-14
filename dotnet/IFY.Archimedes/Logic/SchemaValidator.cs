using IFY.Archimedes.Models;
using IFY.Archimedes.Models.Schema;
using IFY.Archimedes.Models.Schema.Json;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace IFY.Archimedes.Logic;

public partial class SchemaValidator
{
    /// <summary>
    /// Gets the collection of schema validation errors encountered during processing.
    /// </summary>
    public List<SchemaError> Errors { get; } = [];
    public bool HasFailed => Errors.Any(e => e.Level == SchemaError.ErrorLevel.Error);

    public Dictionary<string, ArchComponent>? Result { get; private set; }

    private JsonConfig? _config;
    private readonly Dictionary<string, JsonComponent> _schema = [];

    public bool AddSchema(string path)
    {
        // Read input
        if (!File.Exists(path))
        {
            Errors.Add(new SchemaError(SchemaError.ErrorLevel.Error, $"File not found: {path}"));
            return false;
        }
        var input = File.ReadAllText(path);

        // Ensure input is JSON
        switch (new FileInfo(path).Extension.ToLower())
        {
            case ".json":
            case ".jsonc":
                break;
            case ".yaml":
            case ".yml":
                var deserializer = new DeserializerBuilder().Build();
                var yaml = deserializer.Deserialize(new StringReader(input))
                    ?? throw new InvalidDataException($"Failed to parse YAML: {path}");
                input = JsonSerializer.Serialize(yaml);
                break;
            default:
                throw new InvalidDataException($"Unsupported file type: {path}");
        }

        // Parse input
        Dictionary<string, JsonElement>? parsed;
        try
        {
            parsed = Utility.DeserializeJson<Dictionary<string, JsonElement>>(input);
            if (parsed is null)
            {
                Errors.Add(new(SchemaError.ErrorLevel.Error, $"Failed to parse file: {path}"));
                return false;
            }
        }
        catch (JsonException jex)
        {
            Errors.Add(new(SchemaError.ErrorLevel.Error, $"Failed to parse file: {jex.Message}", jex));
            return false;
        }

        // Merge source
        foreach (var key in parsed.Keys)
        {
            if (_schema.ContainsKey(key)
            {
                Errors.Add(new(SchemaError.ErrorLevel.Error, $"Duplicate key in incoming files: {key}"));
                return false;
            }

            if (!JsonComponent.TryParse(key, parsed[key], out var comp, out var errs))
            {
                Errors.AddRange(errs);
            }
            else
            {
                _schema[key] = comp;
            }
        }

        return !HasFailed;
    }

    /// <summary>
    /// Returns flattened structure of components with validated properties
    /// from the current schema.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(Result))]
    public bool Validate()
    {
        if (HasFailed)
        {
            return false;
        }

        var all = new Dictionary<string, ArchComponent>();

        // Ensure all components are unique and valid
        foreach (var comp in _schema)
        {
            parseComponents(comp.Key, comp.Value);
        }

        // Validate and resolve all links
        foreach (var comp in all.Values)
        {
            foreach (var link in comp.Source.Links)
            {
                if (!all.ContainsKey(link.Key))
                {
                    throw new InvalidDataException($"Component '{comp.Id}' has a link to undefined target '{link.Key}'.");
                }

                var newLink = new Link(comp.Id, link.Key, LinkType.Default, null, false);

                // Can be string or JsonLink
                string? typeValue = null;
                if (link.Value.ValueKind == JsonValueKind.String)
                {
                    typeValue = link.Value.GetString();
                }
                else if (link.Value.ValueKind == JsonValueKind.Object)
                {
                    var obj = link.Value.Deserialize<JsonLink>(Utility.GlobalJsonOptions);
                    if (obj is null)
                    {
                        Errors.Add(new(SchemaError.ErrorLevel.Error, $"Unable to deserialise link object for component '{comp.Id}' and link '{link.Key}'."));
                    }
                    else
                    {
                        typeValue = obj.Type;
                        newLink = newLink with
                        {
                            Text = obj.Text,
                            Reverse = obj.Reverse
                        };
                    }
                }
                else
                {
                    Errors.Add(new(SchemaError.ErrorLevel.Error, $"Unable to deserialise link object for component '{comp.Id}' and link '{link.Key}'."));
                }

                if (!Enum.TryParse(typeValue ?? "default", true, out LinkType linkType))
                {
                    Errors.Add(new(SchemaError.ErrorLevel.Error, $"Component '{comp.Id}' has an invalid link type '{link.Value}' for target '{link.Key}'."));
                }
                comp.Links.Add(newLink with { Type = linkType });
            }
        }

        Result = all;
        return true;

        ArchComponent parseComponents(string key, JsonComponent item, ArchComponent? parent = null)
        {
            // Validate key format: ^\w+$
            if (!ComponentIdFormat().IsMatch(key))
            {
                Errors.Add(new(SchemaError.ErrorLevel.Error, $"Component key '{key}' is invalid. Keys must be non-empty and contain only letters, numbers, and underscores."));
            }

            if (all.ContainsKey(key))
            {
                Errors.Add(new(SchemaError.ErrorLevel.Error, $"Component '{key}' is defined multiple times."));
            }

            var nodeType = NodeType.Default;
            if (item.Type?.Length > 0 && !Enum.TryParse(item.Type, true, out nodeType))
            {
                Errors.Add(new(SchemaError.ErrorLevel.Error, $"Component '{key}' has an invalid 'Type' property: {item.Type}"));
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
