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

    public LoggerBuilder WriteToRotatingFile(string path, ILogFormatter? formatter = null, 
        long maxFileSize = 100 * 1024 * 1024, int maxFiles = 7, bool compress = false)
    {
        _config.Targets.Add(new RotatingFileTarget(path, formatter, maxFileSize, maxFiles, compress));
        return this;
    }

    public LoggerBuilder WriteToJsonFile(string path)
    {
        _config.Targets.Add(new JsonFileTarget(path));
        return this;
    }

    public LoggerBuilder WriteToRotatingJsonFile(string path, 
        long maxFileSize = 100 * 1024 * 1024, int maxFiles = 7, bool compress = false)
    {
        _config.Targets.Add(new RotatingFileTarget(path, new JsonFormatter(), maxFileSize, maxFiles, compress));
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

    public LoggerBuilder OnlyWhen(Func<bool> condition)
    {
        if (_config.Targets.Count <= 0) return this;
        var lastTarget = _config.Targets[^1];
        _config.Targets[^1] = new ConditionalTarget(lastTarget, condition);
        return this;
    }

    public LoggerBuilder OnlyInDevelopment()
    {
        return OnlyWhen(EnvironmentHelper.IsDevelopment);
    }

    public LoggerBuilder OnlyInProduction()
    {
        return OnlyWhen(EnvironmentHelper.IsProduction);
    }

    public LoggerBuilder OnlyInStaging()
    {
        return OnlyWhen(EnvironmentHelper.IsStaging);
    }

    public LoggerBuilder OnlyInEnvironment(string environmentName)
    {
        return OnlyWhen(() => EnvironmentHelper.GetEnvironment().Equals(environmentName, StringComparison.OrdinalIgnoreCase));
    }

    public LoggerBuilder WriteToConsoleInDevelopment(ILogFormatter? formatter = null)
    {
        return WriteToConsole(formatter).OnlyInDevelopment();
    }

    public LoggerBuilder WriteToFileInProduction(string path, ILogFormatter? formatter = null)
    {
        return WriteToFile(path, formatter).OnlyInProduction();
    }

    public LoggerBuilder WriteToRotatingFileInProduction(string path, ILogFormatter? formatter = null,
        long maxFileSize = 100 * 1024 * 1024, int maxFiles = 7, bool compress = false)
    {
        return WriteToRotatingFile(path, formatter, maxFileSize, maxFiles, compress).OnlyInProduction();
    }

    public PineLogger Build()
    {
        return new PineLogger(_config);
    }
}