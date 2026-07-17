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
}