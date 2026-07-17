using Microsoft.Playwright;

public class CorporateWellnessPage
{
    private readonly IPage _page;

    private ILocator NameInput => _page.Locator("#name").First;
    private ILocator OrganizationNameInput => _page.Locator("#organizationName").First;
    private ILocator ContactNumberInput => _page.Locator("#contactNumber").First;
    private ILocator EmailInput => _page.Locator("#officialEmailId").First;
    private ILocator ScheduleDemoButton => _page.Locator("button[type='submit']").First;

    public CorporateWellnessPage(IPage page)
    {
        _page = page;
    }

    // Generic helper: fills any given field, then blurs by clicking a
    // different field, so validation triggers consistently for every case.
    private async Task FillAndBlurAsync(ILocator field, string value, ILocator blurTarget)
    {
        await field.FillAsync(value);
        await blurTarget.ClickAsync();
    }

    public Task EnterContactNumberAsync(string value) =>
        FillAndBlurAsync(ContactNumberInput, value, NameInput);

    public Task EnterEmailAsync(string value) =>
        FillAndBlurAsync(EmailInput, value, NameInput);

    public Task EnterNameAsync(string value) =>
        FillAndBlurAsync(NameInput, value, OrganizationNameInput);

    public Task EnterOrganizationNameAsync(string value) =>
        FillAndBlurAsync(OrganizationNameInput, value, NameInput);

    public async Task<bool> HasErrorClassAsync(ILocator field)
    {
        var classAttribute = await field.GetAttributeAsync("class");
        return classAttribute?.Contains("corporate-form__input--error") ?? false;
    }

    public Task<bool> IsContactNumberInvalidAsync() => HasErrorClassAsync(ContactNumberInput);
    public Task<bool> IsEmailInvalidAsync() => HasErrorClassAsync(EmailInput);

    public async Task<bool> IsSubmitButtonDisabledAsync()
    {
        return await ScheduleDemoButton.IsDisabledAsync();
    }

    public async Task TypeThenClearFieldAsync(ILocator field, ILocator blurTarget)
    {
        await field.FillAsync("a");
        await field.FillAsync("");
        await blurTarget.ClickAsync();
    }

    public Task TypeThenClearNameAsync() =>
        TypeThenClearFieldAsync(NameInput, OrganizationNameInput);

    public Task<bool> IsNameInvalidAsync() => HasErrorClassAsync(NameInput);
}