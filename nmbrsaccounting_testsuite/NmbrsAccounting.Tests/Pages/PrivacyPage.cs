using NmbrsAccounting.Tests.Support;

namespace NmbrsAccounting.Tests.Pages;

/// <summary>
/// Page object for the Privacy page. Inherits navigation logic from BasePage.
/// Demonstrates code reuse through inheritance - reuses OpenMenuPageAsync() and other
/// shared navigation methods from BasePage without duplicating code.
/// </summary>
public sealed class PrivacyPage : BasePage
{
    // CSS selectors matching the Privacy page markup in Privacy.cshtml
    private const string ContentSelector = "p";     // The paragraph content

    // Constructor receives PlaywrightDriver via Reqnroll's dependency injection
    public PrivacyPage(PlaywrightDriver driver) : base(driver) { }

    /// <summary>
    /// Extracts the main content text from the paragraph element.
    /// </summary>
    public async Task<string> GetContentAsync()
        => (await Page.Locator(ContentSelector).InnerTextAsync()).Trim();
}
