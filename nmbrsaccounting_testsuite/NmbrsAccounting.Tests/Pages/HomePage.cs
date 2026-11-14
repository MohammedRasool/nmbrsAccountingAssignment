using NmbrsAccounting.Tests.Support;

namespace NmbrsAccounting.Tests.Pages;

/// <summary>
/// Page object for the Home page. Inherits all menu navigation from BasePage.
/// No page-specific methods needed - all navigation handled by BasePage.
/// </summary>
public sealed class HomePage : BasePage
{
    // Constructor receives PlaywrightDriver via Reqnroll's dependency injection
    public HomePage(PlaywrightDriver driver) : base(driver) { }
}