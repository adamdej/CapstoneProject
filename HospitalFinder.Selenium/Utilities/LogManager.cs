using Serilog;

public static class LogManager
{
    private static ILogger? _logger;

    public static ILogger Logger => _logger ?? throw new InvalidOperationException("Logger has not been initialised.");

    public static void Initialise()
    {
        var projectRoot = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(), "..", "..", ".."));

        var logDirectory = Path.Combine(
            projectRoot,
            ConfigurationManager.Settings.LogDirectory);
        Directory.CreateDirectory(logDirectory);

        _logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File(
                Path.Combine(logDirectory, "test-log-.txt"),
                rollingInterval: RollingInterval.Day,
                shared: true)
            .CreateLogger();
    }

    public static void Dispose()
    {
        Log.CloseAndFlush();
    }
}