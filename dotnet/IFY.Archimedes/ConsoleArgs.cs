namespace IFY.Archimedes;

/// <summary>
/// Provides functionality for parsing command-line arguments into named and positional values.
/// </summary>
/// <remarks>This class distinguishes between named arguments (prefixed with one or more dashes, such as "-name
/// value") and positional arguments (arguments without a leading dash). Named arguments may be parsed as flags (if no
/// value follows) or as key-value pairs. Positional arguments are assigned an incremental position index. Use the Parse
/// method to enumerate parsed arguments as structured records.</remarks>
/// <param name="args">The array of command-line arguments to parse. Each element represents a single argument as passed to the application
/// entry point.</param>
public class ConsoleArgs(string[] args)
{
    /// <summary>
    /// Represents a command-line argument with an optional position, name, and value.
    /// </summary>
    /// <remarks>Use this record to represent both named and positional command-line arguments. Either the
    /// position or the name (or both) may be specified, depending on the argument type.</remarks>
    /// <param name="Position">The zero-based position of the argument in the command-line input, or null if the argument is not positional.</param>
    /// <param name="Name">The name of the argument, or null if the argument is positional and unnamed.</param>
    /// <param name="Value">The value assigned to the argument, or null if no value is specified.</param>
    public record Arg(uint? Position, string? Name, string? Value);

    /// <summary>
    /// Parses the command-line arguments and returns a sequence of argument representations.
    /// </summary>
    /// <remarks>Named arguments are identified by a leading dash ('-'). If a named argument is immediately
    /// followed by a non-dash value, it is treated as a key-value pair; otherwise, it is treated as a flag with no
    /// value. Positional arguments are those without a leading dash. The order of returned arguments matches the order
    /// in the input array.</remarks>
    /// <returns>An enumerable collection of <see cref="Arg"/> objects, each representing a positional argument, named argument,
    /// or flag parsed from the input arguments.</returns>
    public IEnumerable<Arg> Parse()
    {
        var pos = 0u;
        for (var i = 0; i < args.Length; ++i)
        {
            if (args[i][0] == '-')
            {
                var name = args[i].TrimStart('-');

                // Named arg
                if (i + 1 < args.Length && args[i + 1][0] != '-')
                {
                    // TODO: multi-value?
                    yield return new(null, name, args[++i]);
                }

                // Flag
                yield return new(null, name, null);
            }
            else
            {
                yield return new(pos++, null, args[i]);
            }
        }
    }

    // TODO: get by position / name
    // TODO: validation (e.g. required, unique names, valid names, ordered positions, max positions)
    // TODO: help text
}
