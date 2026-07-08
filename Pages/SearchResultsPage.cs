using OpenQA.Selenium;

public class SearchResultsPage : BasePage
{
    // Confirmed via DevTools inspection of https://www.practo.com/bangalore/hospitals
    private By HospitalCardLocator => By.CssSelector(".c-estb-card");
    private By NameLocator => By.CssSelector("h2.line-1");
    private By RatingLocator => By.CssSelector(".c-feedback .u-bold");
    private By StatusLocator => By.CssSelector(".line-4 span");

    public SearchResultsPage(IWebDriver driver) : base(driver) { }

    public bool IsDisplayed() => WaitUtils.WaitForElement(Driver, HospitalCardLocator) != null;

    public List<string> GetHospitalNames()
    {
        return GetCards().Select(card => card.FindElement(NameLocator).Text.Trim()).ToList();
    }

    public List<double> GetHospitalRatings()
    {
        return GetCards().Select(card =>
        {
            var text = card.FindElement(RatingLocator).Text.Trim();
            return double.TryParse(text, out var rating) ? rating : 0.0;
        }).ToList();
    }

    public List<bool> GetOpen24HoursFlags()
    {
        return GetCards().Select(card =>
            card.FindElements(StatusLocator).Any(e => e.Text.Contains("Open 24x7"))
        ).ToList();
    }

    // NOTE: Parking facility is NOT exposed anywhere in Practo's hospital listing data
    // (confirmed via full-card DOM inspection). This method filters by rating and
    // 24x7 status only.
    public List<string> GetFilteredHospitalNames(double minRating, bool requireOpen24Hours)
    {
        var cards = GetCards();
        var results = new List<string>();

        foreach (var card in cards)
        {
            var ratingText = card.FindElement(RatingLocator).Text.Trim();
            var rating = double.TryParse(ratingText, out var r) ? r : 0.0;
            var isOpen24Hours = card.FindElements(StatusLocator).Any(e => e.Text.Contains("Open 24x7"));

            if (rating > minRating && (!requireOpen24Hours || isOpen24Hours))
            {
                results.Add(card.FindElement(NameLocator).Text.Trim());
            }
        }

        return results;
    }

    public HomePage NavigateBackToHome()
    {
        Driver.Navigate().GoToUrl(ConfigurationManager.Settings.BaseUrl);
        return new HomePage(Driver);
    }

    private IReadOnlyList<IWebElement> GetCards()
    {
        WaitUtils.WaitForElement(Driver, HospitalCardLocator);
        return Driver.FindElements(HospitalCardLocator);
    }
}