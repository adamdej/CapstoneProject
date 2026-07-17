using Microsoft.Playwright;
using Reqnroll;

[Binding]
public class PlaywrightHooks
{
    private readonly ScenarioContext _scenarioContext;
    private static IPlaywright? _playwright;
    private static IBrowser? _browser;
    private IBrowserContext? _context;

    public PlaywrightHooks(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    [BeforeTestRun]
    public static async Task BeforeTestRun()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = Environment.GetEnvironmentVariable("HEADED") != "1"
        });
    }

    [BeforeScenario]
    public async Task BeforeScenario()
    {
        _context = await _browser!.NewContextAsync();
        var page = await _context.NewPageAsync();

        // Store the Page in ScenarioContext so step definition classes
        // can retrieve it via constructor injection.
        _scenarioContext["Page"] = page;
    }

    [AfterScenario]
    public async Task AfterScenario()
    {
        if (_context != null)
        {
            await _context.CloseAsync();
        }
    }

    [AfterTestRun]
    public static async Task AfterTestRun()
    {
        if (_browser != null) await _browser.CloseAsync();
        _playwright?.Dispose();
    }
}