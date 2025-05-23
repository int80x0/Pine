using Pine.Formatters;

namespace Pine.Targets;

public class ConsoleTarget : ILogTarget
{
    private readonly ILogFormatter _formatter;
    private readonly Dictionary<LogLevel, ConsoleColor> _colors;

    public ConsoleTarget(ILogFormatter? formatter = null)
    {
        _formatter = formatter ?? new DefaultFormatter();
        _colors = new Dictionary<LogLevel, ConsoleColor>
        {
            [LogLevel.Trace] = ConsoleColor.Gray,
            [LogLevel.Debug] = ConsoleColor.Blue,
            [LogLevel.Info] = ConsoleColor.Green,
            [LogLevel.Warning] = ConsoleColor.Yellow,
            [LogLevel.Error] = ConsoleColor.Red,
            [LogLevel.Fatal] = ConsoleColor.Magenta
        };
    }

    public Task WriteAsync(LogEntry entry)
    {
        var originalColor = Console.ForegroundColor;
        
        if (_colors.TryGetValue(entry.Level, out var color))
        {
            Console.ForegroundColor = color;
        }
        
        var message = _formatter.Format(entry);
        Console.WriteLine(message);
        
        Console.ForegroundColor = originalColor;
        
        return Task.CompletedTask;
    }

    public void Dispose()
    {
    }
}