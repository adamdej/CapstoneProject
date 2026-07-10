using NUnit.Framework;

public class NavigationTests : BaseTest
{
    [Test]
    public void SearchResults_NavigateBack_ReturnsToHomePage()
    {
        LogManager.Logger.Information("Starting SearchResults_NavigateBack test");

        var homePage = new HomePage(Driver!);
        var criteria = TestDataManager.Data.HospitalSearch;

        var resultsPage = homePage.NavigateToHospitalsInCity(criteria.City);

        Assert.That(resultsPage.IsDisplayed(), Is.True,
            "Expected hospital listing cards to be visible before navigating back.");

        var homePageAfterBack = resultsPage.NavigateBackToHome();

        Assert.That(homePageAfterBack.IsDisplayed(), Is.True,
            "Expected to land back on the practo.com home page after navigating back.");

        LogManager.Logger.Information("SearchResults_NavigateBack test passed");
    }

    [Test]
    public void Window_TabHandling()
    {
        LogManager.Logger.Information("Starting SearchResults_NavigateBack test");

        var homePage = new HomePage(Driver!);
        var criteria = TestDataManager.Data.HospitalSearch;

        var resultsPage = homePage.NavigateToHospitalsInCity(criteria.City);

        Assert.That(resultsPage.IsDisplayed(), Is.True,
            "Expected hospital listing cards to be visible before navigating back.");

        var homePageAfterBack = resultsPage.NavigateBackToHome();

        Assert.That(homePageAfterBack.IsDisplayed(), Is.True,
            "Expected to land back on the practo.com home page after navigating back.");
    }
}