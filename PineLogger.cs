using Pine.Configuration;

namespace Pine;

public class PineLogger(PineConfiguration config, string? category = null) : IDisposable
{
    private readonly string? _category = category ?? config.DefaultCategory;

    public static LoggerBuilder Create() => new();

    public PineLogger ForCategory(string category)
    {
        return new PineLogger(config, category);
    }

    public async Task LogAsync(LogLevel level, string message, Exception? exception = null,
        Dictionary<string, object>? properties = null)
    {
        if (level < config.MinimumLevel) return;

        var entry = new LogEntry(
            DateTime.Now,
            level,
            message,
            _category,
            exception,
            properties
        );

        var tasks = config.Targets.Select(target => target.WriteAsync(entry));
        await Task.WhenAll(tasks);
    }

    public void Log(LogLevel level, string message, Exception? exception = null,
        Dictionary<string, object>? properties = null)
    {
        LogAsync(level, message, exception, properties).ConfigureAwait(false);
    }

    public async Task TraceAsync(string message, Dictionary<string, object>? properties = null)
        => await LogAsync(LogLevel.Trace, message, null, properties);

    public async Task DebugAsync(string message, Dictionary<string, object>? properties = null)
        => await LogAsync(LogLevel.Debug, message, null, properties);

    public async Task InfoAsync(string message, Dictionary<string, object>? properties = null)
        => await LogAsync(LogLevel.Info, message, null, properties);

    public async Task WarningAsync(string message, Dictionary<string, object>? properties = null)
        => await LogAsync(LogLevel.Warning, message, null, properties);

    public async Task ErrorAsync(string message, Exception? exception = null,
        Dictionary<string, object>? properties = null)
        => await LogAsync(LogLevel.Error, message, exception, properties);

    public async Task FatalAsync(string message, Exception? exception = null,
        Dictionary<string, object>? properties = null)
        => await LogAsync(LogLevel.Fatal, message, exception, properties);

    public void Trace(string message, Dictionary<string, object>? properties = null)
        => Log(LogLevel.Trace, message, null, properties);

    public void Debug(string message, Dictionary<string, object>? properties = null)
        => Log(LogLevel.Debug, message, null, properties);

    public void Info(string message, Dictionary<string, object>? properties = null)
        => Log(LogLevel.Info, message, null, properties);

    public void Warning(string message, Dictionary<string, object>? properties = null)
        => Log(LogLevel.Warning, message, null, properties);

    public void Error(string message, Exception? exception = null, Dictionary<string, object>? properties = null)
        => Log(LogLevel.Error, message, exception, properties);

    public void Fatal(string message, Exception? exception = null, Dictionary<string, object>? properties = null)
        => Log(LogLevel.Fatal, message, exception, properties);

    public void Dispose()
    {
        foreach (var target in config.Targets)
        {
            target.Dispose();
        }
    }
}