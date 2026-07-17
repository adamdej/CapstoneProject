using Microsoft.Playwright;

public static class ScreenshotUtils
{
    public static async Task<string> TakeScreenshotAsync(IPage page, string testName)
    {
        var projectRoot = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(), "..", "..", ".."));

        var screenshotDirectory = Path.Combine(
            projectRoot,
            ConfigurationManager.Settings.ScreenshotDirectory);
        Directory.CreateDirectory(screenshotDirectory);

        // Sanitize test names up front (handles parameterized tests like
        // Test("Delhi") which contain characters invalid in Windows paths).
        var safeTestName = string.Join("_", testName.Split(Path.GetInvalidFileNameChars()));

        var path = Path.Combine(screenshotDirectory,
            $"{safeTestName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");

        await page.ScreenshotAsync(new PageScreenshotOptions { Path = path });

        return path;
    }
}