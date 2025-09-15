using IFY.Archimedes.Tests.Runners;

namespace IFY.Archimedes.Tests;

[TestClass]
public sealed class LinkTests
{
    [TestMethod]
    [DynamicData(nameof(ILogicRunner.GetRunners), typeof(ILogicRunner))]
    public void Can_link_nodes(ILogicRunner runner)
    {
        // Arrange
        var yaml = @"
A:
  links:
    B: {}
B: {}
";

        // Act
        var result = runner.Run([yaml], out var errors);

        // Assert
        Assert.IsNotNull(result, string.Join(Environment.NewLine, errors));
        Assert.Contains("A --> B", result);
    }

    [TestMethod]
    [DynamicData(nameof(ILogicRunner.GetRunners), typeof(ILogicRunner))]
    public void Can_add_text_to_link(ILogicRunner runner)
    {
        // Arrange
        var yaml = @"
A:
  links:
    B:
      text: A to B
B: {}
";

        // Act
        var result = runner.Run([yaml], out var errors);

        // Assert
        Assert.IsNotNull(result, string.Join(Environment.NewLine, errors));
        Assert.Contains("A -->|\"A to B\"| B", result);
    }

    [TestMethod]
    [DynamicData(nameof(ILogicRunner.GetRunners), typeof(ILogicRunner))]
    public void Can_add_reversed_link(ILogicRunner runner)
    {
        // Arrange
        var yaml = @"
A:
  links:
    B:
      reverse: true
B: {}
";

        // Act
        var result = runner.Run([yaml], out var errors);

        // Assert
        Assert.IsNotNull(result, string.Join(Environment.NewLine, errors));
        Assert.Contains("B --> A", result);
    }

    [TestMethod]
    [DynamicData(nameof(ILogicRunner.GetRunners), typeof(ILogicRunner))]
    public void Can_apply_predefined_style_to_link(ILogicRunner runner)
    {
        // Arrange
        var yaml = @"
A:
  links:
    B: default
    C: dots
    D: line
    E: thick
    F: invisible
B: {}
C: {}
D: {}
E: {}
F: {}
";

        // Act
        var result = runner.Run([yaml], out var errors);

        // Assert
        Assert.IsNotNull(result, string.Join(Environment.NewLine, errors));
        Assert.Contains("A --> B", result);
        Assert.Contains("A -.-> C", result);
        Assert.Contains("A --- D", result);
        Assert.Contains("A ==> E", result);
        Assert.Contains("A ~~~ F", result);
    }
}
