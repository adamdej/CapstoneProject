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

    [SetUp]
    public void SetUp()
    {
        LogManager.Initialise();
        ExtentReportManager.Initialise();
        ExtentReportManager.CreateTest(TestContext.CurrentContext.Test.Name);

        Driver = DriverFactory.Create(
            ConfigurationManager.Settings.Browser,
            ConfigurationManager.Settings.Headless);
        DriverContext.Driver = Driver;

        LogManager.Logger.Information("Browser launched, navigating to {Url}",
            ConfigurationManager.Settings.BaseUrl);
        ExtentReportManager.LogInfo("Browser launched and navigated to base URL");

        Driver.Navigate().GoToUrl(ConfigurationManager.Settings.BaseUrl);
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
                ExtentReportManager.LogFail("Test failed — screenshot captured");
                AllureApi.AddAttachment("Screenshot", "image/png", path);
            }
            else
            {
                ExtentReportManager.LogPass("Test passed");
            }
        }
        finally
        {
            ExtentReportManager.Flush();
            Driver?.Quit();
            Driver?.Dispose();
            LogManager.Dispose();
        }
    }
}