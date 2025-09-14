using IFY.Archimedes.Models.Schema.Json;

namespace IFY.Archimedes.Models.Schema;

public record NodeType
{
    public static readonly NodeType Default = new() { Format = null! };
    public static readonly NodeType Node = new() { Format = "[\"{0}\"]" };
    public static readonly NodeType Block = new() { Format = "[[\"{0}\"]]" };

    public static readonly Dictionary<string, NodeType> Types = new()
    {
        ["default"] = Default,
        ["node"] = Node,
        ["block"] = Block,
        ["data"] = new() { Format = "[(\"{0}\")]" },
        ["pill"] = new() { Format = "([\"{0}\"])" },
        ["soft"] = new() { Format = "(\"{0}\")" },
        ["circle"] = new() { Format = "((\"{0}\"))" },
        ["circle2"] = new() { Format = "(((\"{0}\")))" },
        ["flag"] = new() { Format = ">\"{0}\"]" },
        ["diamond"] = new() { Format = "{\"{0}\"}" },
        ["hexagon"] = new() { Format = "{{\"{0}\"}}" },
        ["lean-r"] = new() { Format = "[/\"{0}\"/]" },
        ["lean-l"] = new() { Format = "[\\\"{0}\"\\]" },
        ["roof"] = new() { Format = "[/\"{0}\"\\]" },
        ["boat"] = new() { Format = "[\\\"{0}\"/]" },
    };

    public JsonNodeStyle? Style { get; set; }
    public required string Format { get; init; }
}