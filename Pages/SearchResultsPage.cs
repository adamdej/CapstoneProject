using OpenQA.Selenium;

public class SearchResultsPage : BasePage
{
    // Confirmed via DevTools inspection of https://www.practo.com/bangalore/hospitals
    private By HospitalCardLocator => By.CssSelector(".c-estb-card");
    private By NameLocator => By.CssSelector("h2.line-1");
    private By RatingLocator => By.CssSelector(".c-feedback .u-bold");
    private By StatusLocator => By.CssSelector(".line-4 span");

    public SearchResultsPage(IWebDriver driver) : base(driver) { }

    // Detects Akamai's bot-management challenge page specifically, as opposed
    // to the real page just still loading. Akamai's own page text says "If
    // this page doesn't refresh automatically, resubmit your request" — this
    // method follows that instruction programmatically rather than attempting
    // to bypass the check itself.
    private bool IsAkamaiChallengePage()
    {
        return Driver.PageSource.Contains("Processing your request")
            || Driver.PageSource.Contains("resubmit your request");
    }

    public bool IsDisplayed(int maxRetries = 2)
    {
        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            // Poll for up to ExplicitWaitSeconds, checking every 500ms for
            // EITHER real content or an Akamai challenge — rather than waiting
            // the full timeout blindly and only checking Akamai afterward.
            var deadline = DateTime.UtcNow.AddSeconds(ConfigurationManager.Settings.ExplicitWaitSeconds);

            while (DateTime.UtcNow < deadline)
            {
                if (Driver.FindElements(HospitalCardLocator).Count > 0)
                {
                    return true;
                }

                if (IsAkamaiChallengePage())
                {
                    LogManager.Logger.Warning(
                        "Akamai challenge page detected, waiting 5s before refresh (attempt {Attempt}/{Max})",
                        attempt + 1, maxRetries);
                    Thread.Sleep(5000);
                    Driver.Navigate().Refresh();
                    break; // exit polling loop, go to next outer attempt
                }

                Thread.Sleep(500);
            }
        }

        return false;
    }

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

    // Returns true if no hospital cards are present — used for negative tests
    // (e.g. invalid city) where the page loads but returns zero results.
    // Uses FindElements (plural) rather than WaitForElement, since FindElements
    // returns an empty list instead of throwing when nothing matches.
    public bool HasNoHospitalCards()
    {
        return !Driver.FindElements(HospitalCardLocator).Any();
    }

    private IReadOnlyList<IWebElement> GetCards()
    {
        WaitUtils.WaitForElement(Driver, HospitalCardLocator);
        return Driver.FindElements(HospitalCardLocator);
    }
}