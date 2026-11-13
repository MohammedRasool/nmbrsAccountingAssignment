using NmbrsAccounting.Tests.Support;

namespace NmbrsAccounting.Tests.Pages;

/// <summary>
/// Page object for the Home page. Inherits all menu navigation from BasePage.
/// The only page-specific method is OpenAsync() which navigates directly to the base URL.
/// </summary>
public sealed class HomePage : BasePage
{
    // Constructor receives PlaywrightDriver via Reqnroll's dependency injection
    public HomePage(PlaywrightDriver driver) : base(driver) { }

    /// <summary>Navigates directly to the application's base URL (https://localhost:5001).</summary>
    public Task OpenAsync() => Driver.OpenHomeAsync();
}