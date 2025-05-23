using System.Text.Json;

namespace Pine.Formatters;

public class JsonFormatter : ILogFormatter
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public string Format(LogEntry entry)
    {
        var obj = new
        {
            timestamp = entry.Timestamp,
            level = entry.Level.ToString().ToLower(),
            message = entry.Message,
            category = entry.Category,
            exception = entry.Exception?.ToString(),
            properties = entry.Properties
        };

        return JsonSerializer.Serialize(obj, Options);
    }
}