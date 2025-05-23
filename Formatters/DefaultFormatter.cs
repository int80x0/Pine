using System.Text;

namespace Pine.Formatters;

public class DefaultFormatter : ILogFormatter
{
    public string Format(LogEntry entry)
    {
        var sb = new StringBuilder();
        
        sb.Append($"[{entry.Timestamp:HH:mm:ss.fff}] ");
        sb.Append($"[{entry.Level.ToString().ToUpper()}] ");
        
        if (!string.IsNullOrEmpty(entry.Category))
        {
            sb.Append($"[{entry.Category}] ");
        }
        
        sb.Append(entry.Message);
        
        if (entry.Exception != null)
        {
            sb.AppendLine();
            sb.Append(entry.Exception.ToString());
        }
        
        if (entry.Properties?.Count > 0)
        {
            sb.AppendLine();
            foreach (var prop in entry.Properties)
            {
                sb.Append($"  {prop.Key}: {prop.Value}");
            }
        }
        
        return sb.ToString();
    }
}