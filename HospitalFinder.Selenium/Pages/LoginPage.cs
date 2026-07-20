using OpenQA.Selenium;

public class LoginPage : BasePage
{
    private By UsernameInput => By.CssSelector("#username");
    private By PasswordInput => By.CssSelector("#password");
    private By UsernameErrorBlock => By.CssSelector("#usernameErrorBlock");
    private By PasswordErrorBlock => By.CssSelector("#passwordErrorBlock");
    private By LoginButton => By.CssSelector("#login");

    public LoginPage(IWebDriver driver) : base(driver) { }

    public void Navigate()
    {
        Driver.Navigate().GoToUrl(
            $"{ConfigurationManager.Settings.BaseUrl.TrimEnd('/')}/login?redirectToPath=https%3A%2F%2Fwww.practo.com%2F");
    }

    // Clicks Login with both fields left empty, triggering required-field
    // validation on both the username and password inputs simultaneously.
    public void ClickLoginWithEmptyFields()
    {
        WaitUtils.WaitForElement(Driver, UsernameInput);
        Driver.FindElement(LoginButton).Click();
    }

    public string GetUsernameErrorText()
    {
        Wait.Until(d => !string.IsNullOrWhiteSpace(d.FindElement(UsernameErrorBlock).Text));
        return Driver.FindElement(UsernameErrorBlock).Text.Trim();
    }

    public string GetPasswordErrorText()
    {
        Wait.Until(d => !string.IsNullOrWhiteSpace(d.FindElement(PasswordErrorBlock).Text));
        return Driver.FindElement(PasswordErrorBlock).Text.Trim();
    }
}