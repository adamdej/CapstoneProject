using Microsoft.Playwright;

public class DiagnosticsPage
{
    private readonly IPage _page;

    // Playwright's Locator is different from Selenium's By/FindElement —
    // a Locator doesn't immediately search the DOM when created; it's a
    // reusable "recipe" for finding elements, re-evaluated fresh every
    // time you call an action on it. This is part of why Playwright
    // doesn't need manual waits: locating and waiting are combined.
    private ILocator TopCityItems => _page.Locator("div.u-bg--white.u-marginb--std > ul.u-br-rule li");

    public DiagnosticsPage(IPage page)
    {
        _page = page;
    }

    public async Task<List<string>> GetTopCitiesAsync()
    {
        // AllTextContentsAsync() waits for at least one matching element,
        // then returns the text of every match as a List<string> directly —
        // no manual loop needed, unlike Selenium's FindElements + Select().
        var cityTexts = await TopCityItems.AllTextContentsAsync();

        // Each <li> contains both an <img alt="image"> and the city name text.
        // TextContentAsync concatenates all text nodes, so we trim whitespace
        // but don't need to strip anything else since the <img> has no
        // visible text content of its own.
        return cityTexts.Select(t => t.Trim()).ToList();
    }
}