using System.Runtime.Serialization;

namespace IFY.Archimedes.Models.Schema;

public enum LinkType
{
    [EnumMember(Value = "-->")]
    Default,
    [EnumMember(Value = "-.->")]
    Dots,
    [EnumMember(Value = "---")]
    Line,
    [EnumMember(Value = "==>")]
    Thick,
    [EnumMember(Value = "~~~")]
    Invisible
}
