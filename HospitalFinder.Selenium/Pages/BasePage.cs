using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

public abstract class BasePage
{
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;

    protected BasePage(IWebDriver driver)
    {
        Driver = driver;
        Wait = new WebDriverWait(driver,
            TimeSpan.FromSeconds(ConfigurationManager.Settings.ExplicitWaitSeconds));
    }

    protected void WaitForUrlToContain(string urlPart)
    {
        Wait.Until(d => d.Url.Contains(urlPart));
    }
}