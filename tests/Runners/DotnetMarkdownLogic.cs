using IFY.Archimedes.Logic;

namespace IFY.Archimedes.Tests.Runners;

public class DotnetMarkdownLogic : ILogicRunner
{
    public string? Run(string[] inputs, out string[] errors)
    {
        var validator = new SchemaValidator();
        foreach (var input in inputs)
        {
            validator.AddYaml(input);
        }

        if (!validator.Validate())
        {
            errors = []; // TODO: validator.Errors.Select().ToArray();
            return null;
        }

        errors = [];
        var diagrams = DiagramBuilder.BuildDiagrams(validator.Result);

        var mermaidWriter = new MermaidWriter(validator.Result, validator.Config);
        var markdownWriter = new MarkdownWriter(validator.Config);
        return markdownWriter.Write(diagrams, mermaidWriter);
    }
}
