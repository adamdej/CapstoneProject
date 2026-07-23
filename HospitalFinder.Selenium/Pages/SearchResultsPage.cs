using OpenQA.Selenium;

public class SearchResultsPage : BasePage
{
    private By HospitalCardLocator => By.CssSelector(".c-estb-card");
    private By NameLocator => By.CssSelector("h2.line-1");
    private By NameLinkLocator => By.XPath(".//a[h2[@class='line-1']]");
    private By RatingLocator => By.CssSelector(".c-feedback .u-bold");
    private By StatusLocator => By.CssSelector(".line-4 span");
    private By ReadMoreInfoLocator => By.CssSelector("[data-qa-id='read_more_info']");
    private By AmenityItemLocator => By.CssSelector("[data-qa-id='amenities_list'] [data-qa-id='amenity_item']");

    public SearchResultsPage(IWebDriver driver) : base(driver) { }

    public bool IsDisplayed() => WaitUtils.WaitForElement(Driver, HospitalCardLocator) != null;

    public bool HasNoHospitalCards()
    {
        return !Driver.FindElements(HospitalCardLocator).Any();
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

    // Represents one hospital card's data, captured BEFORE we navigate away
    // to check individual detail pages for parking.
    private record HospitalCandidate(string Name, double Rating, bool IsOpen24Hours, string DetailUrl);

    private List<HospitalCandidate> GetHospitalCandidates()
    {
        return GetCards().Select(card =>
        {
            var name = card.FindElement(NameLocator).Text.Trim();

            var ratingText = card.FindElement(RatingLocator).Text.Trim();
            var rating = double.TryParse(ratingText, out var r) ? r : 0.0;

            var isOpen24Hours = card.FindElements(StatusLocator).Any(e => e.Text.Contains("Open 24x7"));

            var relativeHref = card.FindElement(NameLinkLocator).GetAttribute("href");

            return new HospitalCandidate(name, rating, isOpen24Hours, relativeHref);
        }).ToList();
    }

    // Checks a single hospital's detail page for a "Parking" amenity.
    // Navigates the CURRENT tab directly to the detail URL (rather than
    // clicking the card, which opens target="_blank") since we already
    // have the href captured — no need to juggle multiple windows here.
    private bool HasParkingAmenity(string detailUrl)
    {
        Driver.Navigate().GoToUrl(detailUrl);

        var readMoreButton = WaitUtils.WaitForElement(Driver, ReadMoreInfoLocator);
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", readMoreButton);

        // A cookie-consent overlay (class="fc-dialog-overlay") intermittently
        // intercepts native clicks on Chrome/Edge, though not observed on Firefox.
        // A JS-triggered click bypasses Selenium's overlap check, same pattern
        // used earlier for the "Book Hospital Visit" button.
        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", readMoreButton);

        WaitUtils.WaitForElement(Driver, AmenityItemLocator);
        var amenities = Driver.FindElements(AmenityItemLocator);

        return amenities.Any(a => a.Text.Trim().Equals("Parking", StringComparison.OrdinalIgnoreCase));
    }

    // Filters by rating, 24x7 status, AND parking availability (checked via
    // each hospital's individual detail page — Practo does not expose
    // parking data on the listing cards themselves).
    public List<string> GetFilteredHospitalNames(double minRating, bool requireOpen24Hours, bool requireParking)
    {
        var candidates = GetHospitalCandidates()
            .Where(c => c.Rating > minRating && (!requireOpen24Hours || c.IsOpen24Hours))
            .ToList();

        if (!requireParking)
        {
            return candidates.Select(c => c.Name).ToList();
        }

        var results = new List<string>();
        foreach (var candidate in candidates)
        {
            if (HasParkingAmenity(candidate.DetailUrl))
            {
                results.Add(candidate.Name);
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

    public bool IsDisplayed(int maxRetries = 2)
    {
        for (int attempt = 0; attempt <= maxRetries; attempt++)
        {
            var deadline = DateTime.UtcNow.AddSeconds(10);

            while (DateTime.UtcNow < deadline)
            {
                if (Driver.FindElements(HospitalCardLocator).Count > 0)
                {
                    return true;
                }

                if (IsAkamaiChallengePage())
                {
                    Thread.Sleep(5000);
                    Driver.Navigate().Refresh();
                    break;
                }

                Thread.Sleep(500);
            }
        }

        return false;
    }

    // Detects Akamai's bot-management challenge — either the soft, timed
    // countdown page, or the harder "Challenge Validation" tier seen from
    // data-center/CI networks.
    private bool IsAkamaiChallengePage()
    {
        return Driver.PageSource.Contains("Processing your request")
            || Driver.PageSource.Contains("resubmit your request")
            || Driver.Title.Contains("Challenge Validation");
    }
}

