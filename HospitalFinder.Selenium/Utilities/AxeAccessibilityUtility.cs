using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
using OpenQA.Selenium;
using NUnit.Framework;

public static class AxeAccessibilityUtility
{
    private static readonly string[] WcagTags =
    [
        "wcag2a", "wcag2aa", "wcag21a", "wcag21aa", "wcag22a", "wcag22aa"
    ];

    public static AxeResult ScanEntirePage(IWebDriver driver, string? outputFile = null)
    {
        AxeBuilder builder = new AxeBuilder(driver).WithTags(WcagTags);

        if (!string.IsNullOrWhiteSpace(outputFile))
        {
            CreateParentDirectory(outputFile);
            builder.WithOutputFile(outputFile);
        }

        return builder.Analyze();
    }

    public static void AssertNoViolations(AxeResult result)
    {
        Assert.That(result.Violations, Is.Empty,
            $"Accessibility violations found: {result.Violations.Length}");
    }

    private static void CreateParentDirectory(string filePath)
    {
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }
}