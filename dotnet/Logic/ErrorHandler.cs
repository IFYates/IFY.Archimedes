namespace IFY.Archimedes.Logic;

public static class ErrorHandler
{
    public static void Warning(string message, Exception? ex = null)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("[Warning] " + message);
        Console.ResetColor();
    }

    public static void Error(string message, Exception? ex = null)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("[Error] " + message);
        if (ex != null)
        {
            Console.WriteLine(ex);
        }
        Console.ResetColor();
    }
}
