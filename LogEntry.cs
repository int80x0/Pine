namespace Pine;

public readonly record struct LogEntry(
    DateTime Timestamp,
    LogLevel Level,
    string Message,
    string? Category,
    Exception? Exception,
    Dictionary<string, object>? Properties
);