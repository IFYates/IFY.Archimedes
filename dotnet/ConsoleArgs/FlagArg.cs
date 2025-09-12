namespace IFY.Archimedes.ConsoleArgs;

public class FlagArg(string name, Action handler, string? description = null)
    : ConsoleArg(name, false, description)
{
    public Action Handler { get; } = handler;
}
