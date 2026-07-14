using NUnit.Framework;

public class NegativeTests : BaseTest
{
    // Negative test: verifies filtering degrades gracefully when no hospital can
    // possibly satisfy the criteria (rating > 4.9), rather than throwing an
    // exception or silently returning incorrect data.
    [Test]
    public void ImpossibleFilterCriteria()
    {
        var criteria = TestDataManager.Data.HospitalSearch;
        LogManager.Logger.Information("Starting hospital search for {City}", criteria.City);

        var homePage = new HomePage(Driver!);
        var resultsPage = homePage.NavigateToHospitalsInCity(criteria.City);

        Assert.That(resultsPage.IsDisplayed(), Is.True,
            "Expected hospital listing cards to be visible for the requested city.");

        var hospitalNames = resultsPage.GetFilteredHospitalNames(
            criteria.MaxRating,
            criteria.RequireOpen24Hours);

        Assert.That(hospitalNames, Is.Empty,
            $"Expected no hospitals in {criteria.City} with rating < "
            + $"{criteria.MaxRating}{(criteria.RequireOpen24Hours ? " and Open 24x7" : "")}");
    }

    // Negative test: verifies the framework handles a nonexistent city gracefully
    // (zero hospital cards found) rather than throwing an unhandled exception,
    // using FindElements rather than the exception-throwing WaitForElement pattern.
    [Test]
    public void Invalid_NonexistantCityTest()
    {
        var criteria = TestDataManager.Data.HospitalSearch;
        LogManager.Logger.Information("Starting hospital search for {City}", criteria.City);

        var homePage = new HomePage(Driver!);
        var resultsPage = homePage.NavigateToHospitalsInCity("Notarealcityxyz");

        Assert.That(resultsPage.HasNoHospitalCards(), Is.True,
            "Expected no hospital cards to be found for a nonexistent city.");
    }
}