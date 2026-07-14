using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

public static class ExtentReportManager
{
    private static ExtentReports? _extent;

    [ThreadStatic]
    private static ExtentTest? _test;

    public static void Initialise()
    {
        if (_extent != null) return;

        var projectRoot = Path.GetFullPath(Path.Combine(
            Directory.GetCurrentDirectory(), "..", "..", ".."));

        var reportDirectory = Path.Combine(
            projectRoot,
            ConfigurationManager.Settings.ExtentReportDirectory);
        Directory.CreateDirectory(reportDirectory);

        var reportPath = Path.Combine(reportDirectory, "extent-report.html");
        var reporter = new ExtentSparkReporter(reportPath);
        reporter.Config.DocumentTitle = "HospitalFinder Test Report";
        reporter.Config.ReportName = "Automation Results";

        _extent = new ExtentReports();
        _extent.AttachReporter(reporter);
    }

    public static ExtentTest CreateTest(string testName)
    {
        _test = _extent!.CreateTest(testName);
        return _test;
    }

    public static void LogPass(string message) => _test?.Pass(message);
    public static void LogFail(string message) => _test?.Fail(message);
    public static void LogInfo(string message) => _test?.Info(message);
    public static void Flush() => _extent?.Flush();
}