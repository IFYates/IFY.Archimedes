using System.Runtime.Serialization;

namespace IFY.Archimedes.Models.Schema;

public enum NodeType
{
    Default,
    [EnumMember(Value = "[\"{0}\"]")]
    Node,
    [EnumMember(Value = "[[\"{0}\"]]")]
    Block,
    [EnumMember(Value = "[(\"{0}\")]")]
    Data,
    [EnumMember(Value = "([\"{0}\"])")]
    Pill,
    [EnumMember(Value = "(\"{0}\")")]
    Soft,
    [EnumMember(Value = "((\"{0}\"))")]
    Circle,
    [EnumMember(Value = "(((\"{0}\")))")]
    Circle2,
    [EnumMember(Value = ">\"{0}\"]")]
    Flag,
    [EnumMember(Value = "{\"{0}\"}")]
    Diamond,
    [EnumMember(Value = "{{\"{0}\"}}")]
    Hexagon,
    [EnumMember(Value = "[/\"{0}\"/]")]
    LeanR,
    [EnumMember(Value = "[\\\"{0}\"\\]")]
    LeanL,
    [EnumMember(Value = "[/\"{0}\"\\]")]
    Roof,
    [EnumMember(Value = "[\\\"{0}\"/]")]
    Boat
}