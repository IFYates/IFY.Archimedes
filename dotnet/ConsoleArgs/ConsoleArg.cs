namespace IFY.Archimedes.ConsoleArgs;

public abstract class ConsoleArg(string name, bool required, string? description)
{
    public string Name { get; } = name;
    //public bool NoValue { get; } = noValue;
    public bool Required { get; } = required;
    public string? Description { get; } = description;
}
