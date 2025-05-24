using Pine.Formatters;
using System.Collections.Concurrent;
using System.IO.Compression;

namespace Pine.Targets;

public class RotatingFileTarget : ILogTarget
{
    private readonly ILogFormatter _formatter;
    private readonly string _baseFilePath;
    private readonly long _maxFileSize;
    private readonly int _maxFiles;
    private readonly bool _compress;
    private readonly ConcurrentQueue<string> _queue;
    private readonly Timer _flushTimer;
    private readonly SemaphoreSlim _semaphore;
    private string _currentFilePath;
    private long _currentFileSize;

    public RotatingFileTarget(string baseFilePath, ILogFormatter? formatter = null, 
        long maxFileSize = 100 * 1024 * 1024, // 100MB default
        int maxFiles = 7, 
        bool compress = false)
    {
        _baseFilePath = baseFilePath;
        _maxFileSize = maxFileSize;
        _maxFiles = maxFiles;
        _compress = compress;
        _formatter = formatter ?? new DefaultFormatter();
        _queue = new ConcurrentQueue<string>();
        _semaphore = new SemaphoreSlim(1, 1);
        
        Directory.CreateDirectory(Path.GetDirectoryName(_baseFilePath)!);
        
        _currentFilePath = _baseFilePath;
        _currentFileSize = File.Exists(_currentFilePath) ? new FileInfo(_currentFilePath).Length : 0;
        
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
                await WriteMessages(messages);
            }
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task WriteMessages(List<string> messages)
    {
        var content = string.Join(Environment.NewLine, messages) + Environment.NewLine;
        var contentBytes = System.Text.Encoding.UTF8.GetByteCount(content);

        if (_currentFileSize + contentBytes > _maxFileSize)
        {
            await RotateFile();
        }

        await File.AppendAllTextAsync(_currentFilePath, content);
        _currentFileSize += contentBytes;
    }

    private async Task RotateFile()
    {
        var directory = Path.GetDirectoryName(_baseFilePath)!;
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(_baseFilePath);
        var extension = Path.GetExtension(_baseFilePath);
        var timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss");
        
        var rotatedFileName = $"{fileNameWithoutExt}.{timestamp}{extension}";
        var rotatedFilePath = Path.Combine(directory, rotatedFileName);

        if (File.Exists(_currentFilePath))
        {
            File.Move(_currentFilePath, rotatedFilePath);
            
            if (_compress)
            {
                await CompressFile(rotatedFilePath);
            }
        }

        _currentFilePath = _baseFilePath;
        _currentFileSize = 0;
        
        await CleanupOldFiles();
    }

    private async Task CompressFile(string filePath)
    {
        try
        {
            var compressedPath = filePath + ".gz";

            await using var originalStream = File.OpenRead(filePath);
            await using var compressedStream = File.Create(compressedPath);
            await using var gzipStream = new GZipStream(compressedStream, CompressionMode.Compress);
            
            await originalStream.CopyToAsync(gzipStream);
            
            File.Delete(filePath);
        }
        catch (Exception)
        {
            // If compression fails, keep the original file
        }
    }

    private async Task CleanupOldFiles()
    {
        try
        {
            var directory = Path.GetDirectoryName(_baseFilePath)!;
            var fileNameWithoutExt = Path.GetFileNameWithoutExtension(_baseFilePath);
            var extension = Path.GetExtension(_baseFilePath);
            
            var pattern = $"{fileNameWithoutExt}.????-??????{extension}*";
            var rotatedFiles = Directory.GetFiles(directory, pattern)
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .ToList();

            var filesToDelete = rotatedFiles.Skip(_maxFiles);
            
            foreach (var file in filesToDelete)
            {
                try
                {
                    File.Delete(file.FullName);
                }
                catch
                {
                    // ignore
                }
            }
        }
        catch
        {
            // ignore
        }
    }

    public void Dispose()
    {
        _flushTimer?.Dispose();
        FlushQueue(null);
        _semaphore?.Dispose();
    }
}