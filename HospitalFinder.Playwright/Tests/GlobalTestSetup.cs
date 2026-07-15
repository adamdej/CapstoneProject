using NUnit.Framework;

// [SetUpFixture] runs exactly ONCE for the entire test assembly, before any
// test's [SetUp] runs and after all tests finish. This ensures
// ExtentReportManager.Initialise() happens exactly once, and Flush() happens
// exactly once at the very end — avoiding the parallel file-write race
// condition we discovered and fixed in the Selenium project.
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