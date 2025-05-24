namespace Pine.Targets;

public class ConditionalTarget(ILogTarget target, Func<bool> condition) : ILogTarget
{
    public Task WriteAsync(LogEntry entry)
    {
        return condition() ? target.WriteAsync(entry) : Task.CompletedTask;
    }

    public void Dispose()
    {
        target.Dispose();
    }
}

// Environment helper
public static class EnvironmentHelper
{
    public static bool IsDevelopment() => 
        Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true ||
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true;

    public static bool IsProduction() => 
        Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")?.Equals("Production", StringComparison.OrdinalIgnoreCase) == true ||
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Production", StringComparison.OrdinalIgnoreCase) == true;

    public static bool IsStaging() => 
        Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")?.Equals("Staging", StringComparison.OrdinalIgnoreCase) == true ||
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Staging", StringComparison.OrdinalIgnoreCase) == true;

    public static string GetEnvironment() => 
        Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? 
        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? 
        "Production";
}