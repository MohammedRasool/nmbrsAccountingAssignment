using Microsoft.Playwright;
using NmbrsAccounting.Tests.Support;

namespace NmbrsAccounting.Tests.Pages;

/// <summary>
/// Abstract base class for all page objects. Provides shared menu navigation
/// that all pages inherit. Selectors match the Bootstrap layout in _Layout.cshtml.
/// 
/// KEY DESIGN: Menu items are read from the DOM at runtime, so if a developer
/// adds a new page to the navbar, the navigation test automatically covers it
/// without any code changes.
/// </summary>
public abstract class BasePage
{
    // CSS selectors matching _Layout.cshtml's Bootstrap navbar
    protected const string MenuLinksSelector = ".navbar-nav a.nav-link";  // All menu links
    protected const string MainContentSelector = "main[role='main']";     // Main content area

    protected readonly PlaywrightDriver Driver;
    protected IPage Page => Driver.Page;  // Shortcut to the current browser tab

    protected BasePage(PlaywrightDriver driver) => Driver = driver;

    /// <summary>
    /// Reads menu items directly from the DOM at runtime.
    /// Returns e.g. ["Home", "Invoices", "Privacy"].
    /// 
    /// WHY THIS MATTERS: If a developer adds a "Clients" link to the navbar,
    /// this method automatically returns it - no code changes needed.
    /// </summary>
    public async Task<IReadOnlyList<string>> GetMenuLabelsAsync()
    {
        await EnsureAppIsLoadedAsync();  // Navigate to app if on blank page
        
        // Find all menu links, extract their text content
        return (await Page.Locator(MenuLinksSelector).AllTextContentsAsync())
            .Select(t => t.Trim())       // Remove whitespace
            .Where(t => t.Length > 0)    // Filter out empty strings
            .ToList();
    }

    /// <summary>
    /// Clicks a menu link by its text label and waits for the navigation to complete.
    /// Returns the HTTP response so callers can verify the page loaded successfully.
    /// 
    /// FLOW:
    ///   1. Find the link with matching text
    ///   2. Set up a response listener BEFORE clicking (to capture the navigation)
    ///   3. Click the link
    ///   4. Wait for the response and DOM to load
    /// </summary>
    public async Task<IResponse> OpenMenuPageAsync(string label)
    {
        await EnsureAppIsLoadedAsync();

        // Find the menu link with the specified text
        var link = Page.Locator(MenuLinksSelector)
            .Filter(new LocatorFilterOptions { HasText = label })
            .First;

        // IMPORTANT: Set up the listener BEFORE clicking, otherwise we might miss the response
        var documentResponse = Page.WaitForResponseAsync(
            response => response.Request.IsNavigationRequest       // Only navigation requests (not API calls)
                        && response.Request.ResourceType == "document");  // HTML documents only

        await link.ClickAsync();  // Click triggers the navigation

        var response = await documentResponse;  // Wait for and capture the response
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);  // Wait for page to render
        return response;  // Caller can check response.Ok for HTTP 200
    }

    /// <summary>
    /// Checks if the main content area is visible. Used to verify a page loaded successfully.
    /// </summary>
    public async Task<bool> MainContentIsVisibleAsync()
    {
        await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        return await Page.Locator(MainContentSelector).IsVisibleAsync();
    }

    /// <summary>
    /// Extracts the page title from the h1 element.
    /// Used to verify each page displays the correct heading.
    /// </summary>
    public async Task<string> GetPageTitleAsync()
        => (await Page.Locator("h1").InnerTextAsync()).Trim();

    /// <summary>
    /// Ensures the app is loaded before interacting with the menu.
    /// 
    /// WHY: Each scenario starts on about:blank. This method navigates to the
    /// app's base URL if we're not already on it, so menu interactions work.
    /// </summary>
    private async Task EnsureAppIsLoadedAsync()
    {
        if (!Page.Url.StartsWith(PlaywrightDriver.BaseUrl, StringComparison.OrdinalIgnoreCase))
            await Driver.OpenHomeAsync();  // Navigate to https://localhost:5001
    }
}