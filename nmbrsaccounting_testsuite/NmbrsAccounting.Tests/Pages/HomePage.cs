using NmbrsAccounting.Tests.Support;

namespace NmbrsAccounting.Tests.Pages;

/// <summary>
/// Page object for the Home page. Inherits all menu navigation from BasePage.
/// Demonstrates code reuse through inheritance - reuses OpenMenuPageAsync() and other
/// shared navigation methods from BasePage without duplicating code.
/// 
/// Currently no page-specific methods needed - all functionality inherited from BasePage.
/// This class maintains consistency with the Page Object Model pattern and provides
/// a foundation for future Home-specific tests if needed.
/// </summary>
public sealed class HomePage : BasePage
{
    // Constructor receives PlaywrightDriver via Reqnroll's dependency injection
    public HomePage(PlaywrightDriver driver) : base(driver) { }
}