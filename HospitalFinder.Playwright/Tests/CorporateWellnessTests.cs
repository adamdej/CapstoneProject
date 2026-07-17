using NUnit.Framework;

public class CorporateWellnessTests : BaseTest
{
    [Test]
    public async Task CorporateWellnessForm_InvalidContactNumber_ShowsErrorAndDisablesSubmit()
    {
        await Page.GotoAsync("https://www.practo.com/plus/corporate");

        var corporatePage = new CorporateWellnessPage(Page);
        var testData = TestDataManager.Data.CorporateWellnessForm;

        await corporatePage.EnterContactNumberAsync(testData.InvalidContactNumber);

        Assert.Multiple(async () =>
        {
            Assert.That(await corporatePage.IsContactNumberInvalidAsync(), Is.True,
                "Expected the contact number field to be marked invalid for a malformed number.");

            Assert.That(await corporatePage.IsSubmitButtonDisabledAsync(), Is.True,
                "Expected the submit button to remain disabled while the form has invalid data.");
        });

        LogManager.Logger.Information("Confirmed invalid contact number keeps field marked invalid and submit disabled");
    }

    // Verifies the email field follows the same inline-validation pattern as
    // contact number: invalid format triggers the error class and keeps the
    // submit button disabled.
    [Test]
    public async Task CorporateWellnessForm_InvalidEmailAddress_ShowsErrorAndDisablesSubmit()
    {
        await Page.GotoAsync("https://www.practo.com/plus/corporate");
        var corporatePage = new CorporateWellnessPage(Page);
        var testData = TestDataManager.Data.CorporateWellnessForm;
        await corporatePage.EnterEmailAsync(testData.InvalidEmail);
        Assert.Multiple(async () =>
        {
            Assert.That(await corporatePage.IsEmailInvalidAsync(), Is.True,
                "Expected the email field to be marked invalid for a malformed email address.");
            Assert.That(await corporatePage.IsSubmitButtonDisabledAsync(), Is.True,
                "Expected the submit button to remain disabled while the form has invalid data.");
        });
        LogManager.Logger.Information("Confirmed invalid email address keeps field marked invalid and submit disabled");
    }

    // Verifies the name field uses "touch then leave empty" validation rather
    // than validating on page load — an empty field only becomes marked invalid
    // once the user has interacted with it (typed and cleared), confirmed via
    // manual investigation before writing this test.
    [Test]
    public async Task CorporateWellnessForm_EmptyName_ShowsErrorAndDisablesSubmit()
    {
        await Page.GotoAsync("https://www.practo.com/plus/corporate");
        var corporatePage = new CorporateWellnessPage(Page);
        await corporatePage.TypeThenClearNameAsync();
        Assert.Multiple(async () =>
        {
            Assert.That(await corporatePage.IsNameInvalidAsync(), Is.True,
            "Expected the name field to be marked invalid when left empty after being touched.");

            Assert.That(await corporatePage.IsSubmitButtonDisabledAsync(), Is.True,
                "Expected the submit button to remain disabled when the name field is empty.");
        });
        LogManager.Logger.Information("Confirmed empty name field keeps field marked invalid and submit disabled");
    }


    // Verifies organizationName follows the same touch-then-empty validation
    // pattern as name.
    [Test]
    public async Task CorporateWellnessForm_EmptyOrganizationName_ShoeErrorandDisablesSubmit()
    {
        await Page.GotoAsync("https://www.practo.com/plus/corporate");
        var corporatePage = new CorporateWellnessPage(Page);
        await corporatePage.TypeThenClearOrganizationNameAsync();
        Assert.Multiple(async () =>
        {
            Assert.That(await corporatePage.IsOrganizationNameInvalidAsync(), Is.True,
                "Expected the organization name field to be marked invalid when left empty.");
            Assert.That(await corporatePage.IsSubmitButtonDisabledAsync(), Is.True,
                "Expected the submit button to remain disabled when the organization name field is empty.");
        });
        LogManager.Logger.Information("Confirmed empty organization name field keeps submit disabled");
    }
}