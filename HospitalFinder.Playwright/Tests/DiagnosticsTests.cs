using NUnit.Framework;

public class DiagnosticsTests : BaseTest
{
    // Scrapes Practo's Diagnostics page "TOP CITIES" modal into a List<string>.
    // This page is subject to the same intermittent Akamai bot-management
    // blocking observed throughout the Selenium suite — confirmed here as a
    // site-level characteristic, not something specific to either framework.

    [Test]
    public async Task DiagnosticsPage_TopCities_ReturnsExpectedCityList()
    {
        // Note: we navigate directly here rather than relying on BaseTest's
        // SetUp navigation to the homepage, since this test needs the
        // /tests page specifically.
        await Page.GotoAsync("https://www.practo.com/tests");

        var diagnosticsPage = new DiagnosticsPage(Page);
        var cities = await diagnosticsPage.GetTopCitiesAsync();

        LogManager.Logger.Information("Top cities found: {Cities}", string.Join(", ", cities));

        Assert.Multiple(() =>
        {
            Assert.That(cities, Has.Count.EqualTo(8), "Expected exactly 8 top cities.");
            Assert.That(cities, Does.Contain("Bangalore"));
            Assert.That(cities, Does.Contain("Delhi"));
            Assert.That(cities, Does.Contain("Mumbai"));
        });
    }
}