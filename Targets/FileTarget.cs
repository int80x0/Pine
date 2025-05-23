using Pine.Formatters;
using System.Collections.Concurrent;

namespace Pine.Targets;

public class FileTarget : ILogTarget
{
    private readonly ILogFormatter _formatter;
    private readonly string _filePath;
    private readonly ConcurrentQueue<string> _queue;
    private readonly Timer _flushTimer;
    private readonly SemaphoreSlim _semaphore;

    public FileTarget(string filePath, ILogFormatter? formatter = null)
    {
        _filePath = filePath;
        _formatter = formatter ?? new DefaultFormatter();
        _queue = new ConcurrentQueue<string>();
        _semaphore = new SemaphoreSlim(1, 1);
        
        Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
        
        _flushTimer = new Timer(FlushQueue, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
    }

    public Task WriteAsync(LogEntry entry)
    {
        var message = _formatter.Format(entry);
        _queue.Enqueue(message);
        return Task.CompletedTask;
    }

    private async void FlushQueue(object? state)
    {
        if (_queue.IsEmpty) return;

        await _semaphore.WaitAsync();
        try
        {
            var messages = new List<string>();
            while (_queue.TryDequeue(out var message))
            {
                messages.Add(message);
            }

            if (messages.Count > 0)
            {
                await File.AppendAllLinesAsync(_filePath, messages);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public void Dispose()
    {
        _flushTimer?.Dispose();
        FlushQueue(null);
        _semaphore?.Dispose();
    }
}