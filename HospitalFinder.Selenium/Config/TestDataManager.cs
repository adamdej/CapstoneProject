using Microsoft.Extensions.Configuration;

public static class TestDataManager
{
    private static readonly TestData _data;

    static TestDataManager()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("testdata.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        _data = config.GetSection("TestData").Get<TestData>()
            ?? throw new InvalidOperationException("TestData section missing from testdata.json");
    }

    public static TestData Data => _data;
}