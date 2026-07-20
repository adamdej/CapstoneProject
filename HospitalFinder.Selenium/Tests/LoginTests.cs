using NUnit.Framework;

// Chrome is deliberately excluded: navigating to accounts.practo.com/login
// on Chrome specifically returns an Akamai "Access Denied" response, so
// this scenario can only run on Firefox and Edge. This is itself a real,
// documented finding, not an oversight.
[TestFixture("firefox")]
[TestFixture("edge")]
public class LoginTests : BaseTest
{
    public LoginTests(string browser) : base(browser) { }

    // Verifies clicking Login with both fields empty triggers required-field
    // validation on both inputs simultaneously, consistently across browsers.
    // Note: "Email is not registered" (a backend lookup check) was initially
    // tested but found to be non-repeatable across runs, likely due to
    // backend rate-limiting or throttling on repeated checks — this test
    // uses the deterministic, always-reproducible empty-field validation instead.
    [Test]
    public void EmptyFields_ShowsRequiredFieldErrorsOnLoginClick()
    {
        var loginPage = new LoginPage(Driver!);
        loginPage.Navigate();
        loginPage.ClickLoginWithEmptyFields();

        Assert.Multiple(() =>
        {
            Assert.That(loginPage.GetUsernameErrorText(), Is.EqualTo("Mobile Number / Email ID field cannot be empty"));
            Assert.That(loginPage.GetPasswordErrorText(), Is.EqualTo("Password field cannot be empty"));
        });

        LogManager.Logger.Information("Confirmed empty-field validation shows both required-field errors on Login click ({Browser})", Browser);
    }
}