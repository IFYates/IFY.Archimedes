namespace IFY.Archimedes.Tests.Runners;

public interface ILogicRunner
{
    string? Run(string[] inputs, out string[] errors);

    public static IEnumerable<ILogicRunner> GetRunners() =>
    [
        new DotnetMarkdownLogic()
    ];
}
