using OpenQA.Selenium;

public static class ScreenshotUtils
{
    public static string TakeScreenshot(IWebDriver driver, string testName)
    {
        var projectRoot = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(), "..", "..", ".."));

        var screenshotDirectory = Path.Combine(
            projectRoot,
            ConfigurationManager.Settings.ScreenshotDirectory);
        Directory.CreateDirectory(screenshotDirectory);

        var path = Path.Combine(screenshotDirectory,
            $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

        var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
        screenshot.SaveAsFile(path);

        return path;
    }
}