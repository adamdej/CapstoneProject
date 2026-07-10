using NUnit.Framework;

// [SetUpFixture] runs exactly ONCE for the entire test assembly, regardless
// of how many tests run or how much parallelism happens between them.
// This fixes a race condition where multiple parallel tests each calling
// ExtentReportManager.Flush() in their own TearDown collided while writing
// to the same extent-report.html file, causing an intermittent IOException.
[SetUpFixture]
public class GlobalTestSetup
{
    [OneTimeSetUp]
    public void GlobalSetUp()
    {
        ExtentReportManager.Initialise();
    }

    [OneTimeTearDown]
    public void GlobalTearDown()
    {
        ExtentReportManager.Flush();
    }
}