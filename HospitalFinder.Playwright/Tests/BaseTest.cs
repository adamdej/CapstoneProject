using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Allure.Net.Commons;
using Allure.NUnit;


public abstract class BaseTest : PageTest
{
    // Note: Page, Context, and Browser are already provided by PageTest —
    // we don't create or manage them ourselves.

    [SetUp]
    public async Task BaseSetUp()
    {
        LogManager.Initialise();
        ExtentReportManager.CreateTest(TestContext.CurrentContext.Test.Name);

        await Page.GotoAsync(ConfigurationManager.Settings.BaseUrl, new PageGotoOptions { Timeout = 60000 });

        LogManager.Logger.Information("Browser launched and navigated to {Url}",
            ConfigurationManager.Settings.BaseUrl);
        ExtentReportManager.LogInfo("Browser launched and navigated to base URL");
    }

    [TearDown]
    public async Task BaseTearDown()
    {
        try
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;

            if (status == TestStatus.Failed)
            {
                LogManager.Logger.Error("Test failed: {TestName}",
                    TestContext.CurrentContext.Test.Name);

                var path = await ScreenshotUtils.TakeScreenshotAsync(Page,
                    TestContext.CurrentContext.Test.Name);

                TestContext.AddTestAttachment(path);
                LogManager.Logger.Information("Screenshot saved to {Path}", path);
                ExtentReportManager.LogFail("Test failed — screenshot captured");
            }
            else
            {
                ExtentReportManager.LogPass("Test passed");
            }
        }
        finally
        {
            LogManager.Dispose();
        }
    }

    // PageTest's own BrowserNewContextOptions override lets us control
    // headless mode and browser choice via our config, rather than
    // Playwright's defaults.
    public override BrowserNewContextOptions ContextOptions()
    {
        return new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true,
        };
    }
}