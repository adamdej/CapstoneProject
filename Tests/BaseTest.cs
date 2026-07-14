using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Allure.Net.Commons;
using Allure.NUnit;

[TestFixture]
[AllureNUnit]
[Parallelizable(ParallelScope.Fixtures)]
public abstract class BaseTest
{
    protected IWebDriver? Driver;
    private readonly string? _browser;

    // Default constructor: used by test classes that DON'T specify [TestFixture("browser")]
    // — these fall back to whatever "Browser" is set to in appsettings.json, same as before.
    protected BaseTest() { }

    // Parameterized constructor: used by test classes decorated with
    // [TestFixture("chrome")], [TestFixture("firefox")], [TestFixture("edge")] —
    // NUnit creates one instance of the class per attribute, each with its own
    // browser value, and (with [Parallelizable(ParallelScope.Fixtures)]) runs
    // all of them concurrently.
    protected BaseTest(string browser)
    {
        _browser = browser;
    }

    [SetUp]
    public void SetUp()
    {
        var browserToUse = _browser ?? ConfigurationManager.Settings.Browser;

        if (!ConfigurationManager.Settings.UseGrid && browserToUse.ToLowerInvariant() != "chrome")
        {
            Assert.Ignore($"{browserToUse} requires Selenium Grid (only Chrome is available for direct local/CI execution).");
        }
        LogManager.Initialise();
        ExtentReportManager.CreateTest(TestContext.CurrentContext.Test.Name);

        var browserToUse = _browser ?? ConfigurationManager.Settings.Browser;

        Driver = DriverFactory.Create(
            browserToUse,
            ConfigurationManager.Settings.Headless);
        DriverContext.Driver = Driver;

        Driver.Navigate().GoToUrl(ConfigurationManager.Settings.BaseUrl);

        LogManager.Logger.Information("Browser launched ({Browser}) and navigated to {Url}",
            browserToUse, ConfigurationManager.Settings.BaseUrl);
        ExtentReportManager.LogInfo($"Browser launched ({browserToUse}) and navigated to base URL");

        // Dismiss any unexpected browser alert on load
        try
        {
            Driver.SwitchTo().Alert().Accept();
        }
        catch (NoAlertPresentException)
        {
            // No alert present, continue
        }
    }

    [TearDown]
    public void TearDown()
    {
        try
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;

            if (status == TestStatus.Failed)
            {
                LogManager.Logger.Error("Test failed: {TestName}",
                    TestContext.CurrentContext.Test.Name);

                var path = ScreenshotUtils.TakeScreenshot(Driver!,
                    TestContext.CurrentContext.Test.Name);

                TestContext.AddTestAttachment(path);
                LogManager.Logger.Information("Screenshot saved to {Path}", path);
                ExtentReportManager.LogFail("Test failed — screenshot captured");

                AllureApi.AddAttachment("Screenshot", "image/png", path);

                var projectRoot = Path.GetFullPath(Path.Combine(
                    Directory.GetCurrentDirectory(), "..", "..", ".."));
                var logPath = Path.Combine(
                    projectRoot,
                    ConfigurationManager.Settings.LogDirectory,
                    $"test-log-{DateTime.Now:yyyyMMdd}.txt");
                if (File.Exists(logPath))
                {
                    try
                    {
                        using var stream = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                        using var reader = new StreamReader(stream);
                        var logContent = reader.ReadToEnd();

                        AllureApi.AddAttachment("Log", "text/plain",
                            System.Text.Encoding.UTF8.GetBytes(logContent),
                            ".txt");
                    }
                    catch (IOException)
                    {
                        // Log file was locked by another parallel test at the moment of read.
                        // Non-critical — the screenshot and test failure are still captured.
                    }
                }
            }
            else
            {
                ExtentReportManager.LogPass("Test passed");
            }
        }
        finally
        {
            Driver?.Quit();
            Driver?.Dispose();
            LogManager.Dispose();
        }
    }
}