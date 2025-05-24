using Pine.Targets;

namespace Pine.sample;

public class Program
{
    public static async Task Main()
    {
        // Create logger with environment-specific configuration
        var logger = PineLogger.Create()
            .MinimumLevel(LogLevel.Debug)
            .WriteToConsoleInDevelopment()  // Only in Development
            .WriteToRotatingFileInProduction("logs/app.log", maxFileSize: 50 * 1024 * 1024, maxFiles: 10, compress: true)  // Only in Production
            .WriteToRotatingJsonFile("logs/app.json", maxFileSize: 25 * 1024 * 1024, maxFiles: 5)
            .WithDefaultCategory("Sample")
            .Build();

        // Hot-path optimized logging - no string formatting if level disabled
        logger.Info("Application started in {Environment} environment", EnvironmentHelper.GetEnvironment());
        
        // Template-based logging (optimized)
        logger.Debug("Processing item {ItemId} for user {UserId}", 123, 456);
        
        // Traditional logging still works
        logger.Warning("This is a warning");

        // Performance test - these will be skipped if Debug level is disabled
        for (int i = 0; i < 1000; i++)
        {
            logger.Debug("Debug message {Counter}", i); // Hot-path optimized
        }

        try
        {
            throw new InvalidOperationException("Test exception");
        }
        catch (Exception ex)
        {
            logger.Error("An error occurred while processing item {ItemId}", 999, ex);
        }

        var userLogger = logger.ForCategory("User");
        userLogger.Info("User {UserId} logged in with username {Username}", 123, "john_doe");

        // Async logging
        await logger.InfoAsync("Async logging works too!");

        // Test different log levels
        logger.Trace("This is trace level");
        logger.Debug("This is debug level");
        logger.Info("This is info level");
        logger.Warning("This is warning level");
        logger.Error("This is error level");
        logger.Fatal("This is fatal level");

        // Structured logging with properties
        logger.Info("Order processed", new Dictionary<string, object>
        {
            ["OrderId"] = "ORD-001",
            ["CustomerId"] = 12345,
            ["Amount"] = 99.99m,
            ["Currency"] = "USD"
        });

        logger.Dispose();
        
        Console.WriteLine($"Running in {EnvironmentHelper.GetEnvironment()} environment");
        Console.WriteLine("Check logs/ directory for rotating log files");
    }
}