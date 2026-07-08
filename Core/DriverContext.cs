using OpenQA.Selenium;

public static class DriverContext
{
    [ThreadStatic]
    private static IWebDriver? _driver;

    public static IWebDriver Driver
    {
        get => _driver ?? throw new InvalidOperationException("Driver has not been initialised.");
        set => _driver = value;
    }

    public static void Quit()
    {
        _driver?.Quit();
        _driver?.Dispose();
        _driver = null;
    }
}