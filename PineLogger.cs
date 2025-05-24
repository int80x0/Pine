using Pine.Configuration;
using System.Runtime.CompilerServices;

namespace Pine;

public class PineLogger(PineConfiguration config, string? category = null) : IDisposable
{
    private readonly string? _category = category ?? config.DefaultCategory;

    public static LoggerBuilder Create() => new();

    public PineLogger ForCategory(string category)
    {
        return new PineLogger(config, category);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsEnabled(LogLevel level) => level >= config.MinimumLevel;

    public async Task LogAsync(LogLevel level, string message, Exception? exception = null,
        Dictionary<string, object>? properties = null)
    {
        if (!IsEnabled(level)) return;

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
        if (!IsEnabled(level)) return;
        
        _ = Task.Run(async () => await LogAsync(level, message, exception, properties));
    }

    public void Log<T>(LogLevel level, string template, T arg, Exception? exception = null)
    {
        if (!IsEnabled(level)) return;
        Log(level, string.Format(template, arg), exception);
    }

    public void Log<T1, T2>(LogLevel level, string template, T1 arg1, T2 arg2, Exception? exception = null)
    {
        if (!IsEnabled(level)) return;
        Log(level, string.Format(template, arg1, arg2), exception);
    }

    public void Log<T1, T2, T3>(LogLevel level, string template, T1 arg1, T2 arg2, T3 arg3, Exception? exception = null)
    {
        if (!IsEnabled(level)) return;
        Log(level, string.Format(template, arg1, arg2, arg3), exception);
    }

    public async Task TraceAsync(string message, Dictionary<string, object>? properties = null)
    {
        if (!IsEnabled(LogLevel.Trace)) return;
        await LogAsync(LogLevel.Trace, message, null, properties);
    }

    public async Task DebugAsync(string message, Dictionary<string, object>? properties = null)
    {
        if (!IsEnabled(LogLevel.Debug)) return;
        await LogAsync(LogLevel.Debug, message, null, properties);
    }

    public async Task InfoAsync(string message, Dictionary<string, object>? properties = null)
    {
        if (!IsEnabled(LogLevel.Info)) return;
        await LogAsync(LogLevel.Info, message, null, properties);
    }

    public async Task WarningAsync(string message, Dictionary<string, object>? properties = null)
    {
        if (!IsEnabled(LogLevel.Warning)) return;
        await LogAsync(LogLevel.Warning, message, null, properties);
    }

    public async Task ErrorAsync(string message, Exception? exception = null,
        Dictionary<string, object>? properties = null)
    {
        if (!IsEnabled(LogLevel.Error)) return;
        await LogAsync(LogLevel.Error, message, exception, properties);
    }

    public async Task FatalAsync(string message, Exception? exception = null,
        Dictionary<string, object>? properties = null)
    {
        if (!IsEnabled(LogLevel.Fatal)) return;
        await LogAsync(LogLevel.Fatal, message, exception, properties);
    }

    public void Trace(string message, Dictionary<string, object>? properties = null)
    {
        if (!IsEnabled(LogLevel.Trace)) return;
        Log(LogLevel.Trace, message, null, properties);
    }

    public void Debug(string message, Dictionary<string, object>? properties = null)
    {
        if (!IsEnabled(LogLevel.Debug)) return;
        Log(LogLevel.Debug, message, null, properties);
    }

    public void Info(string message, Dictionary<string, object>? properties = null)
    {
        if (!IsEnabled(LogLevel.Info)) return;
        Log(LogLevel.Info, message, null, properties);
    }

    public void Warning(string message, Dictionary<string, object>? properties = null)
    {
        if (!IsEnabled(LogLevel.Warning)) return;
        Log(LogLevel.Warning, message, null, properties);
    }

    public void Error(string message, Exception? exception = null, Dictionary<string, object>? properties = null)
    {
        if (!IsEnabled(LogLevel.Error)) return;
        Log(LogLevel.Error, message, exception, properties);
    }

    public void Fatal(string message, Exception? exception = null, Dictionary<string, object>? properties = null)
    {
        if (!IsEnabled(LogLevel.Fatal)) return;
        Log(LogLevel.Fatal, message, exception, properties);
    }

    public void Trace<T>(string template, T arg) => Log(LogLevel.Trace, template, arg);
    public void Debug<T>(string template, T arg) => Log(LogLevel.Debug, template, arg);
    public void Info<T>(string template, T arg) => Log(LogLevel.Info, template, arg);
    public void Warning<T>(string template, T arg) => Log(LogLevel.Warning, template, arg);
    public void Error<T>(string template, T arg, Exception? exception = null) => Log(LogLevel.Error, template, arg, exception);
    public void Fatal<T>(string template, T arg, Exception? exception = null) => Log(LogLevel.Fatal, template, arg, exception);

    public void Trace<T1, T2>(string template, T1 arg1, T2 arg2) => Log(LogLevel.Trace, template, arg1, arg2);
    public void Debug<T1, T2>(string template, T1 arg1, T2 arg2) => Log(LogLevel.Debug, template, arg1, arg2);
    public void Info<T1, T2>(string template, T1 arg1, T2 arg2) => Log(LogLevel.Info, template, arg1, arg2);
    public void Warning<T1, T2>(string template, T1 arg1, T2 arg2) => Log(LogLevel.Warning, template, arg1, arg2);
    public void Error<T1, T2>(string template, T1 arg1, T2 arg2, Exception? exception = null) => Log(LogLevel.Error, template, arg1, arg2, exception);
    public void Fatal<T1, T2>(string template, T1 arg1, T2 arg2, Exception? exception = null) => Log(LogLevel.Fatal, template, arg1, arg2, exception);

    public void Dispose()
    {
        foreach (var target in config.Targets)
        {
            target.Dispose();
        }
    }
}