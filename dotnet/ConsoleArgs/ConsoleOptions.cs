namespace IFY.Archimedes.ConsoleArgs;

/// <summary>
/// Handles parsing of console arguments against a set of defined arguments.
/// </summary>
public class ConsoleOptions
{
    public ConsoleArg[] Args { get; }
    public PositionalArg[] PositionArgs { get; }

    public ConsoleOptions(params ConsoleArg[] args)
    {
        // TODO: validate argHandlers (e.g. unique names, valid names, ordered positions)
        args.ToString();

        PositionArgs = args.OfType<PositionalArg>().OrderBy(a => a.Position).ToArray();
        Args = args;
    }

    public bool TryParse(string[] args)
    {
        var requiredArgs = Args.Where(a => a.Required).ToHashSet();

        var pos = 0;
        for (var i = 0; i < args.Length; ++i)
        {
            if (args[i][0] == '-') // Named arg
            {
                var name = args[i][1..];
                var arg = Args.FirstOrDefault(a => a.Name == name);
                if (arg == null)
                {
                    // TODO: error
                    return false; // Unknown arg
                }

                if (arg is FlagArg flagArg)
                {
                    flagArg.Handler();
                    requiredArgs.Remove(flagArg);
                }
                else if (arg is NamedArg namedArg)
                {
                    if (i + 1 >= args.Length || args[i + 1][0] == '-')
                    {
                        // TODO: error
                        return false; // Missing value
                    }
                    namedArg.Handler(args[++i]);
                    requiredArgs.Remove(namedArg);
                }
            }
            else // Positional arg
            {
                if (pos >= PositionArgs.Length)
                {
                    // TODO: error
                    return false; // Too many positional args
                }

                PositionArgs[pos].Handler(args[i]);
                requiredArgs.Remove(PositionArgs[pos]);

                ++pos;
            }
        }

        if (requiredArgs.Count > 0)
        {
            // TODO: error
            return false; // Missing required args
        }

        return true;
    }
}
