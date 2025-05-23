namespace Pine.sample;

public class Program
{
    public static async Task Main()
    {

        var logger = PineLogger.Create()
            .MinimumLevel(LogLevel.Debug)
            .WriteToConsole()
            .WriteToFile("logs/app.log")
            .WriteToJsonFile("logs/app.json")
            .WithDefaultCategory("Sample")
            .Build();

        logger.Info("Application started");
        logger.Debug("Debug information");
        logger.Warning("This is a warning");

        try
        {
            throw new InvalidOperationException("Test exception");
        }
        catch (Exception ex)
        {
            logger.Error("An error occurred", ex);
        }

        var userLogger = logger.ForCategory("User");
        userLogger.Info("User logged in", new Dictionary<string, object>
        {
            ["UserId"] = 123,
            ["Username"] = "john_doe"
        });

        await logger.InfoAsync("Async logging works too!!");

        logger.Dispose();
    }
}