public class TestSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Browser { get; set; } = "chromium";
    public bool Headless { get; set; } = true;
    public string ReportType { get; set; } = "extent";
    public string ScreenshotDirectory { get; set; } = "artifacts/screenshots";
    public string ExtentReportDirectory { get; set; } = "artifacts/extent";
    public string LogDirectory { get; set; } = "artifacts/logs";
}