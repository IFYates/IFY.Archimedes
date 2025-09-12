namespace IFY.Archimedes.ConsoleArgs;

public class PositionalArg(uint position, string name, Action<string> handler, bool required = false, string? description = null)
    : NamedArg(name, handler, required, description)
{
    public uint Position { get; } = position;

    public PositionalArg(uint position, string name, Action<string> handler, string? description)
        : this(position, name, handler, false, description)
    { }
}
