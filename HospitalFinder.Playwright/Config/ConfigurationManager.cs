using Microsoft.Extensions.Configuration;

public static class ConfigurationManager
{
    private static readonly TestSettings _settings;

    static ConfigurationManager()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        _settings = config.GetSection("TestSettings").Get<TestSettings>()
            ?? throw new InvalidOperationException("TestSettings section missing from appsettings.json");
    }

    public static TestSettings Settings => _settings;
}