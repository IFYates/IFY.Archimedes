using IFY.Archimedes.Models.Schema;
using IFY.Archimedes.Models.Schema.Json;
using System.Text.Json;
using YamlDotNet.Serialization;

namespace IFY.Archimedes.Logic;

public partial class SchemaValidator
{
    public static readonly string[] ReservedComponentNames =
    [
        "end",
        "linkStyle",
        "style",
        "subgraph"
    ];

    public Dictionary<string, ArchComponent>? Result { get; private set; }
    
    public JsonConfig Config => _config ?? JsonConfig.Default;
    private JsonConfig? _config;

    private readonly Dictionary<string, JsonComponent> _schema = [];

    public bool AddFile(string path)
    {
        // Read input
        if (!File.Exists(path))
        {
            ErrorHandler.Error($"File not found: {path}");
            return false;
        }
        var input = File.ReadAllText(path);

        // Ensure input is JSON
        return new FileInfo(path).Extension.ToLower() switch
        {
            ".js" or ".json" or ".jsonc" => AddJson(input, path),
            ".yaml" or ".yml" => AddYaml(input, path),
            _ => throw new InvalidDataException($"Unsupported file type: {path}"),
        };
    }
    public bool AddYaml(string input, string path = "YAML")
    {
        var deserializer = new DeserializerBuilder().Build();
        var yaml = deserializer.Deserialize(new StringReader(input))
            ?? throw new InvalidDataException($"Failed to parse YAML: {path}");
        var json = JsonSerializer.Serialize(yaml);
        return AddJson(json, path);
    }
    public bool AddJson(string input, string path = "JSON")
    {
        // Parse input
        Dictionary<string, JsonElement>? parsed;
        try
        {
            parsed = Utility.DeserializeJson<Dictionary<string, JsonElement>>(input);
            if (parsed is null)
            {
                ErrorHandler.Error($"Failed to parse file: {path}");
                return false;
            }
        }
        catch (JsonException jex)
        {
            ErrorHandler.Error($"Failed to parse file: {jex.Message}", jex);
            return false;
        }

        // Merge source
        var failed = false;
        var include = new List<string>();
        foreach (var item in parsed)
        {
            switch (item.Key)
            {
                case ".include":
                    if (item.Value.ValueKind == JsonValueKind.String)
                    {
                        var val = item.Value.GetString();
                        if (val != null)
                        {
                            include.Add(val);
                        }
                    }
                    else if (item.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var inc in item.Value.EnumerateArray())
                        {
                            if (inc.ValueKind == JsonValueKind.String)
                            {
                                var val = inc.GetString();
                                if (val != null)
                                {
                                    include.Add(val);
                                }
                            }
                            else
                            {
                                ErrorHandler.Error($"Invalid .include entry in {path}: must be a string or array of strings.");
                                failed = true;
                            }
                        }
                    }
                    else
                    {
                        ErrorHandler.Error($"Invalid .include entry in {path}: must be a string or array of strings.");
                        failed = true;
                    }
                    break;

                case ".config":
                    if (_config != null)
                    {
                        ErrorHandler.Error($"Duplicate key in incoming files: {item.Key}");
                        return false;
                    }
                    if (!JsonConfig.TryParse(item.Value, out _config))
                    {
                        failed = true;
                    }
                    break;

                default:
                    if (ReservedComponentNames.Contains(item.Key))
                    {
                        ErrorHandler.Error($"'{item.Key}' is a reserved word and cannot be used as a component name.");
                        failed = true;
                        break;
                    }
                    if (_schema.ContainsKey(item.Key))
                    {
                        ErrorHandler.Error($"Duplicate key in incoming files: {item.Key}");
                        return false;
                    }
                    if (!JsonComponent.TryParse(item.Key, item.Value, out var comp))
                    {
                        failed = true;
                    }
                    else
                    {
                        _schema[item.Key] = comp;
                    }
                    break;
            }
        }

        // Process includes
        foreach (var inc in include)
        {
            var incPath = Path.IsPathRooted(inc) ? inc : Path.Combine(Path.GetDirectoryName(path) ?? string.Empty, inc);
            failed |= AddFile(incPath);
        }

        return !failed;
    }

    /// <summary>
    /// Returns flattened structure of components with validated properties
    /// from the current schema.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.MemberNotNullWhen(true, nameof(Result))]
    public bool Validate()
    {
        if (_schema.Count == 0)
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
                        ErrorHandler.Error($"Unable to deserialise link object for component '{comp.Id}' and link '{link.Key}'.");
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
                    ErrorHandler.Error($"Unable to deserialise link object for component '{comp.Id}' and link '{link.Key}'.");
                }

                if (!Enum.TryParse(typeValue ?? "default", true, out LinkType linkType))
                {
                    ErrorHandler.Error($"Component '{comp.Id}' has an invalid link type '{link.Value}' for target '{link.Key}'.");
                }
                comp.Links.Add(newLink with { Style = linkType });
            }
        }

        Result = all;
        return true;

        ArchComponent parseComponents(string key, JsonComponent item, ArchComponent? parent = null)
        {
            // Validate key format: ^\w+$
            if (!ComponentIdFormat().IsMatch(key))
            {
                ErrorHandler.Error($"Component key '{key}' is invalid. Keys must be non-empty and contain only letters, numbers, and underscores.");
            }

            if (all.ContainsKey(key))
            {
                ErrorHandler.Error($"Component '{key}' is defined multiple times.");
            }

            // Validate type
            NodeType? nodeType = null;
            if (item.Style?.Length > 0 && !NodeType.Types.TryGetValue(item.Style.ToLower(), out nodeType))
            {
                ErrorHandler.Error($"Unknown node type '{item.Style}' for component '{key}'.");
            }
            nodeType ??= NodeType.Default;

            var comp = new ArchComponent(item, parent, key, nodeType, item.Title?.Length > 0 ? item.Title : key);
            all[key] = comp;

            foreach (var childKey in item.Children.Keys)
            {
                if (ReservedComponentNames.Contains(childKey))
                {
                    ErrorHandler.Error($"'{childKey}' is a reserved word and cannot be used as a component name.");
                    continue;
                }
                if (all.ContainsKey(childKey))
                {
                    ErrorHandler.Error($"Component '{childKey}' is defined multiple times.");
                    continue;
                }

                comp.Children[childKey] = parseComponents(childKey, item.Children[childKey], comp);
            }

            return comp;
        }
    }

    [System.Text.RegularExpressions.GeneratedRegex(@"^\w+$")]
    private static partial System.Text.RegularExpressions.Regex ComponentIdFormat();
}
