using NmbrsAccounting.Tests.Support;

namespace NmbrsAccounting.Tests.Pages;

/// <summary>
/// Page object for the Privacy page. Inherits navigation logic from BasePage.
/// Demonstrates code reuse through inheritance - reuses OpenMenuPageAsync() and other
/// shared navigation methods from BasePage without duplicating code.
/// 
/// Currently no page-specific methods needed - all functionality inherited from BasePage.
/// This class maintains consistency with the Page Object Model pattern and provides
/// a foundation for future Privacy-specific tests if needed.
/// </summary>
public sealed class PrivacyPage : BasePage
{
    // Constructor receives PlaywrightDriver via Reqnroll's dependency injection
    public PrivacyPage(PlaywrightDriver driver) : base(driver) { }
}
