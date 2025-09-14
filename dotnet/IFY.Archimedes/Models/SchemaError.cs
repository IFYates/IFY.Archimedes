namespace IFY.Archimedes.Models;

public record SchemaError(SchemaError.ErrorLevel Level, string Text, Exception? Exception = null)
{
    public enum ErrorLevel
    {
        Warning,
        Error
    }
}
