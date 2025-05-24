# 🌲 Pine Logger

A modern, high-performance logging library for .NET that combines simplicity with powerful features.

[![.NET](https://img.shields.io/badge/.NET-9.0-purple)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
[![NuGet](https://img.shields.io/nuget/v/Pine.svg)](https://www.nuget.org/packages/Pine/)

## ✨ Features

- **🎨 Colorized Console Output** - Beautiful, color-coded log levels
- **📁 Multiple Targets** - Console, File, JSON file support with rotation
- **🔄 Log Rotation** - Automatic file rotation by size with compression
- **⚡ High Performance** - Hot-path optimization and async logging
- **🧵 Thread Safe** - Built for concurrent applications
- **🔧 Flexible Configuration** - Fluent API with environment-specific settings
- **📊 Structured Logging** - JSON format support with custom properties
- **🏷️ Categorized Logging** - Organize logs by category/component
- **🌐 Environment Aware** - Different targets for Development/Production
- **🚀 Modern C#** - Leverages latest .NET 9 features with template-based logging

## 🚀 Quick Start

### Installation

```bash
dotnet add package 80x0.pine
```

### Basic Usage

```csharp
using Pine;

// Create logger with environment-specific configuration
var logger = PineLogger.Create()
    .MinimumLevel(LogLevel.Info)
    .WriteToConsoleInDevelopment()           // Only in Development
    .WriteToRotatingFileInProduction("logs/app.log")  // Only in Production
    .Build();

// High-performance template-based logging
logger.Info("User {UserId} performed {Action}", 123, "Login");
logger.Error("Failed to process order {OrderId}", orderId, exception);

// Cleanup
logger.Dispose();
```

### Environment-Specific Configuration

```csharp
var logger = PineLogger.Create()
    .MinimumLevel(LogLevel.Debug)
    .WriteToConsole().OnlyInDevelopment()
    .WriteToRotatingFile("logs/app.log", maxFileSize: 100_000_000, maxFiles: 7, compress: true).OnlyInProduction()
    .WriteToRotatingJsonFile("logs/structured.json").OnlyInEnvironment("Staging")
    .WithDefaultCategory("MyApp")
    .Build();
```

## ⚡ Performance Features

Pine is optimized for maximum performance:

### Hot-Path Optimization
```csharp
// These calls are optimized - no string formatting if level is disabled
logger.Debug("Processing {ItemId} for {UserId}", itemId, userId);  // ✅ Fast
logger.Debug($"Processing {itemId} for {userId}");                 // ❌ Always allocates
```

### Template-Based Logging
```csharp
// Optimized overloads for common scenarios
logger.Info("User {UserId} action {Action}", 123, "Login");
logger.Error("Failed {Operation} for {Resource}", "Save", "Order", exception);

// Traditional logging still supported
logger.Info("Traditional message", new Dictionary<string, object> { ["Key"] = "Value" });
```

## 📊 Log Rotation

### File Rotation by Size
```csharp
.WriteToRotatingFile("logs/app.log", 
    maxFileSize: 50 * 1024 * 1024,  // 50MB
    maxFiles: 10,                   // Keep 10 files
    compress: true)                 // Compress old files with gzip
```

### JSON File Rotation
```csharp
.WriteToRotatingJsonFile("logs/structured.json", 
    maxFileSize: 25 * 1024 * 1024, 
    maxFiles: 5)
```

Files are rotated with timestamps: `app.20241215-143022.log.gz`

## 🌐 Environment-Aware Logging

Pine automatically detects your environment using `DOTNET_ENVIRONMENT` or `ASPNETCORE_ENVIRONMENT`:

### Conditional Targets
```csharp
// Method 1: Fluent conditions
.WriteToConsole().OnlyInDevelopment()
.WriteToFile("logs/prod.log").OnlyInProduction()
.WriteToRotatingFile("logs/staging.log").OnlyInStaging()

// Method 2: Convenience methods
.WriteToConsoleInDevelopment()
.WriteToRotatingFileInProduction("logs/app.log")

// Method 3: Custom conditions
.WriteToFile("logs/special.log").OnlyWhen(() => Environment.MachineName == "PROD-SERVER")
```

### Environment Detection
```csharp
// Set environment variables:
// DOTNET_ENVIRONMENT=Development
// ASPNETCORE_ENVIRONMENT=Production

EnvironmentHelper.IsDevelopment();  // true/false
EnvironmentHelper.IsProduction();   // true/false  
EnvironmentHelper.GetEnvironment(); // "Development", "Production", etc.
```

## 📊 Log Levels

Pine supports 6 log levels with distinct colors and hot-path optimization:

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
.WriteToConsole()                    // Default formatter with colors
.WriteToConsole(myFormatter)         // Custom formatter
.WriteToConsoleInDevelopment()       // Only in Development
```

### File Targets
```csharp
// Basic file logging
.WriteToFile("logs/app.log") 

// Rotating file with compression
.WriteToRotatingFile("logs/app.log", 
    maxFileSize: 100_000_000,        // 100MB
    maxFiles: 7,                     // Keep 7 files
    compress: true)                  // Gzip compression

// Production-only rotating files
.WriteToRotatingFileInProduction("logs/prod.log", 
    maxFileSize: 500_000_000, maxFiles: 30, compress: true)
```

### JSON File Targets
```csharp
.WriteToJsonFile("logs/app.json")           // Basic JSON logging
.WriteToRotatingJsonFile("logs/app.json",   // Rotating JSON logs
    maxFileSize: 25_000_000, maxFiles: 5)
```

## 🛠️ Advanced Usage

### Category-Based Logging
```csharp
var apiLogger = logger.ForCategory("API");
var dbLogger = logger.ForCategory("Database");

apiLogger.Info("Request {Method} {Path} completed in {Duration}ms", 
    "GET", "/users/123", 45);
dbLogger.Debug("Query executed: {Query}", sql);
```

### Structured Logging
```csharp
logger.Info("Order processed", new Dictionary<string, object>
{
    ["OrderId"] = "ORD-001",
    ["CustomerId"] = 12345,
    ["Amount"] = 99.99m,
    ["ProcessingTimeMs"] = 250,
    ["Items"] = new[] { "Item1", "Item2" }
});
```

### Exception Handling
```csharp
try
{
    await ProcessOrderAsync(order);
}
catch (ValidationException ex)
{
    logger.Warning("Order validation failed for {OrderId}", order.Id, ex);
}
catch (Exception ex)
{
    logger.Error("Unexpected error processing {OrderId}", order.Id, ex);
}
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

## 📈 Performance Benchmarks

Pine's hot-path optimization provides significant performance benefits:

- **🚀 Zero Allocation** - Disabled log levels cause zero allocations
- **⚡ Template Caching** - String formatting is optimized
- **🔄 Async Batching** - File writes are batched for efficiency
- **💾 Smart Buffering** - Automatic flush strategies

```csharp
// Performance comparison
logger.Debug("Processing item {Id}", itemId);        // ✅ ~2ns when disabled
logger.Debug($"Processing item {itemId}");           // ❌ ~50ns always allocates
logger.Debug("Processing item " + itemId);           // ❌ ~80ns always allocates
```

## 🏗️ Architecture

```
Pine/
├── PineLogger.cs              # Main logger with hot-path optimization
├── LogEntry.cs               # Immutable log entry record
├── ILogTarget.cs             # Target interface
├── Targets/                  # Built-in targets
│   ├── ConsoleTarget.cs      # Colorized console output
│   ├── FileTarget.cs         # Basic file writing
│   ├── RotatingFileTarget.cs # File rotation with compression
│   ├── JsonFileTarget.cs     # JSON structured logging
│   └── ConditionalTarget.cs  # Environment-based conditions
├── Formatters/               # Message formatters
│   ├── ILogFormatter.cs      # Formatter interface
│   ├── DefaultFormatter.cs   # Standard text format
│   └── JsonFormatter.cs      # JSON format
└── Configuration/            # Builder pattern setup
    ├── PineConfiguration.cs  # Configuration options
    ├── LoggerBuilder.cs      # Enhanced fluent builder
    └── EnvironmentHelper.cs  # Environment detection
```

## 🔧 Configuration Examples

### Development Setup
```csharp
var logger = PineLogger.Create()
    .MinimumLevel(LogLevel.Debug)
    .WriteToConsoleInDevelopment()
    .WithDefaultCategory("DevApp")
    .Build();
```

### Production Setup
```csharp
var logger = PineLogger.Create()
    .MinimumLevel(LogLevel.Info)
    .WriteToRotatingFileInProduction("logs/app.log", 
        maxFileSize: 100_000_000, maxFiles: 30, compress: true)
    .WriteToRotatingJsonFile("logs/structured.json",
        maxFileSize: 50_000_000, maxFiles: 15)
    .WithDefaultCategory("ProdApp")
    .Build();
```

### Multi-Environment Setup
```csharp
var logger = PineLogger.Create()
    .MinimumLevel(LogLevel.Debug)
    .WriteToConsole().OnlyInDevelopment()
    .WriteToRotatingFile("logs/staging.log").OnlyInStaging()
    .WriteToRotatingFile("logs/production.log", 
        maxFileSize: 200_000_000, maxFiles: 50, compress: true).OnlyInProduction()
    .WriteToRotatingJsonFile("logs/audit.json").OnlyWhen(() => 
        Environment.GetEnvironmentVariable("ENABLE_AUDIT") == "true")
    .WithDefaultCategory("MultiEnvApp")
    .Build();
```

## 📝 Best Practices

1. **Use hot-path optimized methods** - Always use template-based logging for performance
2. **Set appropriate minimum levels** - Use Debug in dev, Info+ in production
3. **Configure environment-specific targets** - Console for dev, files for production
4. **Use log rotation** - Prevent disk space issues with automatic rotation
5. **Include relevant context** - Add properties and categories for better debugging
6. **Handle exceptions properly** - Always pass exception objects to Error/Fatal
7. **Dispose properly** - Call Dispose() or use using statements
8. **Use compression in production** - Save disk space with compressed rotated files

## 🔄 Migration from Other Loggers

### From Serilog
```csharp
// Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log")
    .CreateLogger();

// Pine equivalent
var logger = PineLogger.Create()
    .WriteToConsole()
    .WriteToRotatingFile("logs/app.log")
    .Build();
```

### From NLog
```csharp
// NLog
var logger = LogManager.GetCurrentClassLogger();

// Pine equivalent  
var logger = PineLogger.Create()
    .WithDefaultCategory(nameof(MyClass))
    .WriteToConsoleInDevelopment()
    .WriteToRotatingFileInProduction("logs/app.log")
    .Build();
```

## 🌟 What's New in v1.1

- ⚡ **Hot-path optimization** - Zero allocations for disabled log levels
- 🔄 **Log rotation** - Automatic file rotation with compression
- 🌐 **Environment awareness** - Different targets for different environments
- 📋 **Template logging** - High-performance parameterized messages
- 🎯 **Conditional targets** - Fine-grained control over where logs go
- 🗜️ **Compression support** - Automatic gzip compression for rotated files

## 🤝 Contributing

Contributions are welcome! Please feel free to:

- 🐛 Report bugs
- 💡 Suggest features
- 📝 Improve documentation
- 🔧 Submit pull requests

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🌟 Why Pine?

Pine combines the simplicity you need for quick development with the power required for production applications. With hot-path optimization, automatic log rotation, and environment-aware configuration, Pine scales from development to enterprise.

**Perfect for:**
- ✅ ASP.NET Core applications
- ✅ Console applications
- ✅ Background services
- ✅ Microservices
- ✅ Desktop applications
- ✅ High-performance applications

---

*Happy logging with Pine! 🌲*