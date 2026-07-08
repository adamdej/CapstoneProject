using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public static class WaitUtils
{
    public static IWebElement WaitForElement(IWebDriver driver, By locator)
    {
        var wait = new WebDriverWait(driver,
            TimeSpan.FromSeconds(ConfigurationManager.Settings.ExplicitWaitSeconds));
        return wait.Until(d => d.FindElement(locator));
    }

    public static bool WaitForUrlToContain(IWebDriver driver, string urlPart)
    {
        var wait = new WebDriverWait(driver,
            TimeSpan.FromSeconds(ConfigurationManager.Settings.ExplicitWaitSeconds));
        return wait.Until(d => d.Url.Contains(urlPart));
    }
}