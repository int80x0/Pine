using Pine.Targets;
using Pine.Formatters;

namespace Pine.Configuration;

public class LoggerBuilder
{
    private readonly PineConfiguration _config = new();

    public LoggerBuilder MinimumLevel(LogLevel level)
    {
        _config.MinimumLevel = level;
        return this;
    }

    public LoggerBuilder WriteToConsole(ILogFormatter? formatter = null)
    {
        _config.Targets.Add(new ConsoleTarget(formatter));
        return this;
    }

    public LoggerBuilder WriteToFile(string path, ILogFormatter? formatter = null)
    {
        _config.Targets.Add(new FileTarget(path, formatter));
        return this;
    }

    public LoggerBuilder WriteToJsonFile(string path)
    {
        _config.Targets.Add(new JsonFileTarget(path));
        return this;
    }

    public LoggerBuilder WithDefaultCategory(string category)
    {
        _config.DefaultCategory = category;
        return this;
    }

    public LoggerBuilder AddTarget(ILogTarget target)
    {
        _config.Targets.Add(target);
        return this;
    }

    public PineLogger Build()
    {
        return new PineLogger(_config);
    }
}