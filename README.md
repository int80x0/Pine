# 🌲 Pine Logger

A modern, high-performance logging library for .NET that combines simplicity with powerful features.

[![.NET](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Pine.svg)](https://www.nuget.org/packages/Pine/)

## ✨ Features

- **🎨 Colorized Console Output** - Beautiful, color-coded log levels
- **📁 Multiple Targets** - Console, File, JSON file support
- **⚡ High Performance** - Async logging with batched file writes
- **🧵 Thread Safe** - Built for concurrent applications
- **🔧 Flexible Configuration** - Fluent API for easy setup
- **📊 Structured Logging** - JSON format support with custom properties
- **🏷️ Categorized Logging** - Organize logs by category/component
- **🚀 Modern C#** - Leverages latest .NET 9 features

## 🚀 Quick Start

### Installation

```bash
dotnet add package 80x0.pine
```

### Basic Usage

```csharp
using Pine;

// Create logger with fluent configuration
var logger = PineLogger.Create()
    .MinimumLevel(LogLevel.Info)
    .WriteToConsole()
    .WriteToFile("logs/app.log")
    .Build();

// Log messages
logger.Info("Application started");
logger.Warning("Something might be wrong");
logger.Error("An error occurred", exception);

// Cleanup
logger.Dispose();
```

### Structured Logging

```csharp
// With custom properties
logger.Info("User action", new Dictionary<string, object>
{
    ["UserId"] = 123,
    ["Action"] = "Login",
    ["Timestamp"] = DateTime.Now,
    ["IP"] = "192.16.1.1"
});
```

### Advanced Configuration

```csharp
var logger = PineLogger.Create()
    .MinimumLevel(LogLevel.Debug)
    .WriteToConsole()                    // Colorized console output
    .WriteToFile("logs/app.log")         // Plain text file
    .WriteToJsonFile("logs/data.json")   // Structured JSON
    .WithDefaultCategory("MyApp")
    .Build();

// Category-specific logger
var dbLogger = logger.ForCategory("Database");
dbLogger.Debug("Connection opened");

// Async logging
await logger.InfoAsync("Async operation completed");
```

## 📊 Log Levels

Pine supports 6 log levels with distinct colors:

| Level   | Color   | Description                              |
|---------|---------|------------------------------------------|
| Trace   | Gray    | Detailed diagnostic information          |
| Debug   | Blue    | Debug information for development        |
| Info    | Green   | General informational messages           |
| Warning | Yellow  | Warning messages for potential issues    |
| Error   | Red     | Error messages for failures              |
| Fatal   | Magenta | Critical failures requiring attention    |

## 🎯 Targets

### Console Target
```csharp
.WriteToConsole()              // Default formatter with colors
.WriteToConsole(myFormatter)   // Custom formatter
```

### File Target
```csharp
.WriteToFile("logs/app.log")           // Plain text with batched writes
.WriteToFile("logs/app.log", formatter) // With custom formatter
```

### JSON File Target
```csharp
.WriteToJsonFile("logs/app.json")  // Structured JSON format
```

### Custom Targets
```csharp
public class DatabaseTarget : ILogTarget
{
    public async Task WriteAsync(LogEntry entry)
    {
        // Save to database
        await SaveToDatabase(entry);
    }
    
    public void Dispose() { /* cleanup */ }
}

// Usage
var logger = PineLogger.Create()
    .AddTarget(new DatabaseTarget())
    .Build();
```

## 🛠️ Custom Formatters

Create your own log format:

```csharp
public class CompactFormatter : ILogFormatter
{
    public string Format(LogEntry entry)
    {
        return $"[{entry.Level.ToString().ToUpper()}] {entry.Message}";
    }
}

// Usage
.WriteToConsole(new CompactFormatter())
.WriteToFile("app.log", new CompactFormatter())
```

## 📈 Performance Features

Pine is optimized for high-performance applications:

- **🔄 Async-first Design** - Non-blocking logging operations
- **📦 Batched File Writes** - Multiple log entries written together
- **🚀 Minimal Allocations** - Optimized for low GC pressure
- **⚡ Lock-free Operations** - Concurrent logging without contention
- **💾 Smart Buffering** - Automatic flush on timer and disposal

## 🏗️ Architecture

```
Pine/
├── PineLogger.cs           # Main logger class
├── LogEntry.cs            # Immutable log entry record
├── ILogTarget.cs          # Target interface
├── Targets/               # Built-in targets
│   ├── ConsoleTarget.cs   # Colorized console output
│   ├── FileTarget.cs      # Batched file writing
│   └── JsonFileTarget.cs  # JSON structured logging
├── Formatters/            # Message formatters
│   ├── ILogFormatter.cs   # Formatter interface
│   ├── DefaultFormatter.cs # Standard text format
│   └── JsonFormatter.cs   # JSON format
└── Configuration/         # Builder pattern setup
    ├── PineConfiguration.cs
    └── LoggerBuilder.cs
```

## 🔧 Configuration Options

```csharp
var logger = PineLogger.Create()
    .MinimumLevel(LogLevel.Debug)        // Set minimum log level
    .WriteToConsole()                    // Add console target
    .WriteToFile("logs/app.log")         // Add file target
    .WriteToJsonFile("logs/data.json")   // Add JSON target
    .WithDefaultCategory("MyApp")        // Set default category
    .AddTarget(new CustomTarget())       // Add custom target
    .Build();
```

## 📝 Usage Examples

### Basic Logging
```csharp
logger.Trace("Entering method");
logger.Debug("Processing item {id}", 123);
logger.Info("User logged in successfully");
logger.Warning("API rate limit approaching");
logger.Error("Failed to save data", exception);
logger.Fatal("System critical failure");
```

### Structured Data
```csharp
logger.Info("Order processed", new Dictionary<string, object>
{
    ["OrderId"] = "ORD-001",
    ["CustomerId"] = 12345,
    ["Amount"] = 99.99m,
    ["Currency"] = "USD",
    ["ProcessingTime"] = TimeSpan.FromMilliseconds(245)
});
```

### Category-based Organization
```csharp
var apiLogger = logger.ForCategory("API");
var dbLogger = logger.ForCategory("Database");
var cacheLogger = logger.ForCategory("Cache");

apiLogger.Info("Request received: GET /users");
dbLogger.Debug("Query executed in 15ms");
cacheLogger.Warning("Cache miss for key: user_123");
```

### Exception Handling
```csharp
try
{
    await ProcessOrderAsync(order);
}
catch (ValidationException ex)
{
    logger.Warning("Order validation failed", ex, new Dictionary<string, object>
    {
        ["OrderId"] = order.Id,
        ["ValidationErrors"] = ex.Errors
    });
}
catch (Exception ex)
{
    logger.Error("Unexpected error processing order", ex, new Dictionary<string, object>
    {
        ["OrderId"] = order.Id,
        ["StackTrace"] = ex.StackTrace
    });
}
```

## 🚀 Best Practices

1. **Use appropriate log levels** - Don't log everything as Info
2. **Include context** - Add relevant properties to log entries
3. **Use categories** - Organize logs by component or feature
4. **Handle exceptions** - Always log with exception objects
5. **Dispose properly** - Call Dispose() to flush remaining logs
6. **Async when possible** - Use async methods for better performance

## 🤝 Contributing

Contributions are welcome! Please feel free to:

- 🐛 Report bugs
- 💡 Suggest features
- 📝 Improve documentation
- 🔧 Submit pull requests

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🌟 Why Pine?

Pine combines the simplicity you need for quick development with the power required for production applications. Whether you're building a console app, web API, or enterprise system, Pine scales with your needs.

**Perfect for:**
- ✅ ASP.NET Core applications
- ✅ Console applications
- ✅ Background services
- ✅ Microservices
- ✅ Desktop applications

---

*Happy logging with Pine! 🌲*
