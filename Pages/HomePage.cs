using OpenQA.Selenium;

public class HomePage : BasePage
{
    public HomePage(IWebDriver driver) : base(driver) { }

    public bool IsDisplayed() => Driver.Url.TrimEnd('/') == ConfigurationManager.Settings.BaseUrl.TrimEnd('/');

    public SearchResultsPage NavigateToHospitalsInCity(string city)
    {
        var citySlug = city.Trim().ToLowerInvariant();
        Driver.Navigate().GoToUrl($"{ConfigurationManager.Settings.BaseUrl.TrimEnd('/')}/{citySlug}/hospitals");
        return new SearchResultsPage(Driver);
    }
}