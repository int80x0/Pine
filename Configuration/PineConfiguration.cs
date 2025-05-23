namespace Pine.Configuration;

public class PineConfiguration
{
    public LogLevel MinimumLevel { get; set; } = LogLevel.Info;
    public List<ILogTarget> Targets { get; set; } = [];
    public bool IncludeTimestamp { get; set; } = true;
    public bool IncludeCategory { get; set; } = true;
    public string? DefaultCategory { get; set; }
}