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

        // Parameterized test names (e.g. Test("Delhi")) contain characters like
        // quotes and parentheses that are invalid in Windows file paths.
        var safeTestName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));

        var path = Path.Combine(screenshotDirectory,
            $"{safeTestName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

        var screenshot = ((ITakesScreenshot)driver).GetScreenshot();
        screenshot.SaveAsFile(path);

        return path;
    }
}