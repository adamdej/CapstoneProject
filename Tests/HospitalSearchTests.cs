using NUnit.Framework;

public class HospitalSearchTests : BaseTest
{
    [Test]
    public void BangaloreHospitalSearch_Open24HoursAndMinimumRating_ReturnsMatchingHospitals()
    {
        var criteria = TestDataManager.Data.HospitalSearch;
        LogManager.Logger.Information("Starting hospital search for {City}", criteria.City);

        var homePage = new HomePage(Driver!);
        var resultsPage = homePage.NavigateToHospitalsInCity(criteria.City);

        Assert.That(resultsPage.IsDisplayed(), Is.True,
            "Expected hospital listing cards to be visible for the requested city.");

        var hospitalNames = resultsPage.GetFilteredHospitalNames(
            criteria.MinRating,
            criteria.RequireOpen24Hours);

        Assert.That(hospitalNames, Is.Not.Empty,
            $"Expected at least one hospital in {criteria.City} with rating > "
            + $"{criteria.MinRating}{(criteria.RequireOpen24Hours ? " and Open 24x7" : "")}, but none were found.");

        LogManager.Logger.Information(
            "Found {Count} hospitals matching criteria: {Names}",
            hospitalNames.Count, string.Join(", ", hospitalNames));

        TestContext.Out.WriteLine("Matching hospitals:");
        foreach (var name in hospitalNames)
        {
            TestContext.Out.WriteLine($"- {name}");
        }

        // NOTE: The case study also asks to filter by "parking facility". This was
        // investigated via full DOM inspection of the hospital listing cards and
        // confirmed NOT to be exposed as data anywhere on Practo's hospital pages.
        // This is a documented scope limitation, not an oversight.
    }

    [Test]
    public void GetAllHospitalData_StoresNamesAndRatingsInCollections()
    {
        var criteria = TestDataManager.Data.HospitalSearch;
        var homePage = new HomePage(Driver!);
        var resultsPage = homePage.NavigateToHospitalsInCity(criteria.City);

        Assert.That(resultsPage.IsDisplayed(), Is.True,
            "Expected hospital listing cards to be visible for the requested city.");

        var names = resultsPage.GetHospitalNames();
        var ratings = resultsPage.GetHospitalRatings();

        Assert.Multiple(() =>
        {
            Assert.That(names, Is.Not.Empty);
            Assert.That(ratings, Is.Not.Empty);
            Assert.That(names.Count, Is.EqualTo(ratings.Count),
                "Names and ratings must stay aligned — one rating per hospital name.");
            Assert.That(ratings, Has.All.InRange(0.0, 5.0), "All ratings should be valid star ratings between 0 and 5.");
        });

    }
}