namespace Pine;

public interface ILogTarget
{
    Task WriteAsync(LogEntry entry);
    void Dispose();
}