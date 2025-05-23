namespace Pine.Formatters;

public interface ILogFormatter
{
    string Format(LogEntry entry);
}