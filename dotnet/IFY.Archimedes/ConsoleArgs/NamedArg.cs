namespace IFY.Archimedes.ConsoleArgs;

public class NamedArg(string name, Action<string> handler, bool required = false, string? description = null)
    : ConsoleArg(name, required, description)
{
    public Action<string> Handler { get; } = handler;

    public NamedArg(string name, Action<string> handler, string? description)
        : this(name, handler, false, description)
    { }
}
