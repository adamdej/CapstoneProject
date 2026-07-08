using NUnit.Framework;
using OpenQA.Selenium;

public class AccessibilityTests : BaseTest
{
    [Test]
    public void HospitalSearchResultsPage_HasNoWcagViolations()
    {
        LogManager.Logger.Information("Starting HospitalSearchResultsPage accessibility scan");

        var homePage = new HomePage(Driver!);
        var criteria = TestDataManager.Data.HospitalSearch;
        homePage.NavigateToHospitalsInCity(criteria.City);

        var result = AxeAccessibilityUtility.ScanEntirePage(Driver!);

        AxeAccessibilityUtility.AssertNoViolations(result);

        LogManager.Logger.Information("Accessibility scan complete: {Count} violations found",
            result.Violations.Length);
    }

    [Test]
    public void HospitalSearchInput_IsKeyboardAccessible()
    {
        LogManager.Logger.Information("Starting keyboard-only accessibility check");

        var body = Driver!.FindElement(By.TagName("body"));
        body.SendKeys(Keys.Tab);

        var activeElement = Driver.SwitchTo().ActiveElement();

        Assert.That(activeElement, Is.Not.Null,
            "Expected an element to receive keyboard focus via Tab.");

        LogManager.Logger.Information("Keyboard accessibility check passed");
    }
}