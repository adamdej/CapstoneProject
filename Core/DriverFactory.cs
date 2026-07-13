using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;

public static class DriverFactory
{
    public static IWebDriver Create(string browser, bool headless)
    {
        // Grid mode: instead of launching a browser on THIS machine,
        // create a RemoteWebDriver session against the Selenium Grid hub.
        // The hub forwards the request to whichever node (Chrome/Firefox/Edge)
        // matches the requested browser, and that node launches the browser
        // inside its own Docker container.
        if (ConfigurationManager.Settings.UseGrid)
        {
            return CreateRemoteDriver(browser, headless);
        }

        IWebDriver driver = browser.ToLowerInvariant() switch
        {
            "firefox" => new FirefoxDriver(CreateFirefoxOptions(headless)),
            "edge" => new EdgeDriver(CreateEdgeOptions(headless)),
            _ => new ChromeDriver(CreateChromeOptions(headless))
        };

        driver.Manage().Window.Maximize();
        return driver;
    }

    private static IWebDriver CreateRemoteDriver(string browser, bool headless)
    {
        var hubUri = new Uri(ConfigurationManager.Settings.GridHubUrl);

        // RemoteWebDriver's constructor takes a DriverOptions (the base class
        // that ChromeOptions/FirefoxOptions/EdgeOptions all inherit from),
        // so we can reuse the exact same option-building methods as local runs.
        DriverOptions options = browser.ToLowerInvariant() switch
        {
            "firefox" => CreateFirefoxOptions(headless),
            "edge" => CreateEdgeOptions(headless),
            _ => CreateChromeOptions(headless)
        };

        var driver = new RemoteWebDriver(hubUri, options);
        driver.Manage().Window.Maximize();
        return driver;
    }

    private static ChromeOptions CreateChromeOptions(bool headless)
    {
        var options = new ChromeOptions();
        if (headless) options.AddArgument("--headless=new");
        options.AddArgument("--disable-notifications");
        options.AddArgument("--disable-features=PasswordLeakDetection");

        // Blocks all cookies for the browser session (value 2 = block).
        options.AddUserProfilePreference("profile.default_content_setting_values.cookies", 2);

        // Allows popups/new windows (value 1 = allow).
        options.AddUserProfilePreference("profile.default_content_setting_values.popups", 1);

        return options;
    }

    private static EdgeOptions CreateEdgeOptions(bool headless)
    {
        var options = new EdgeOptions();
        if (headless) options.AddArgument("--headless=new");
        return options;
    }

    private static FirefoxOptions CreateFirefoxOptions(bool headless)
    {
        var options = new FirefoxOptions();
        if (headless) options.AddArgument("-headless");
        return options;
    }
}