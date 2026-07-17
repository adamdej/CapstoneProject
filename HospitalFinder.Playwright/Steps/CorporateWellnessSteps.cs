using Microsoft.Playwright;
using NUnit.Framework;
using Reqnroll;

[Binding]
public class CorporateWellnessSteps
{
    private readonly ScenarioContext _scenarioContext;
    private CorporateWellnessPage? _corporatePage;

    public CorporateWellnessSteps(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
    }

    // Retrieves the Page that PlaywrightHooks.BeforeScenario stored,
    // and wraps it in our existing CorporateWellnessPage object — reusing
    // the exact same page object our plain NUnit tests already use.
    private IPage Page => (IPage)_scenarioContext["Page"];

    [Given(@"I am on the Corporate Wellness page")]
    public async Task GivenIAmOnTheCorporateWellnessPage()
    {
        await Page.GotoAsync("https://www.practo.com/plus/corporate");
        _corporatePage = new CorporateWellnessPage(Page);
    }

    [When(@"I enter an invalid contact number")]
    public async Task WhenIEnterAnInvalidContactNumber()
    {
        var invalidNumber = TestDataManager.Data.CorporateWellnessForm.InvalidContactNumber;
        await _corporatePage!.EnterContactNumberAsync(invalidNumber);
    }

    [Then(@"the contact number field should be marked invalid")]
    public async Task ThenTheContactNumberFieldShouldBeMarkedInvalid()
    {
        Assert.That(await _corporatePage!.IsContactNumberInvalidAsync(), Is.True,
            "Expected the contact number field to be marked invalid.");
    }

    [Then(@"the submit button should remain disabled")]
    public async Task ThenTheSubmitButtonShouldRemainDisabled()
    {
        Assert.That(await _corporatePage!.IsSubmitButtonDisabledAsync(), Is.True,
            "Expected the submit button to remain disabled.");
    }
}