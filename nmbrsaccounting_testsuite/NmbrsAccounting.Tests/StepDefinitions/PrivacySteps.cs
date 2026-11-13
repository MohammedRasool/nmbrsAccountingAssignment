using NmbrsAccounting.Tests.Pages;
using NUnit.Framework;
using Reqnroll;

namespace NmbrsAccounting.Tests.StepDefinitions;

/// <summary>
/// Step definitions for the Privacy.feature file.
/// Binds Gherkin steps to code that interacts with the PrivacyPage object.
/// 
/// SCENARIO: Verify privacy page content
///   Given I am on the "Privacy" page     → Navigate to Privacy page (reuses step from NavigationSteps)
///   Then the page title should be        → Verify h1 contains "Privacy Policy"
///   And the page should contain content  → Verify paragraph contains expected text
/// 
/// This demonstrates step reuse - the "Given I am on the X page" step is defined
/// in NavigationSteps and automatically available for all feature files.
/// </summary>
[Binding]
public sealed class PrivacySteps
{
    private readonly PrivacyPage _privacy;                    // Page object injected via DI
    private string _pageTitle = string.Empty;                 // Stores extracted page title
    private string _pageContent = string.Empty;               // Stores extracted page content

    // Constructor receives PrivacyPage via Reqnroll's dependency injection
    public PrivacySteps(PrivacyPage privacy) => _privacy = privacy;

    /// <summary>
    /// Then the page title should be "{expectedTitle}"
    /// 
    /// Extracts the h1 element text and compares to the expected title.
    /// Stores the result in _pageTitle for potential use in subsequent steps.
    /// </summary>
    [Then("the page title should be \"(.*)\"")]
    public async Task ThenThePageTitleShouldBeAsync(string expectedTitle)
    {
        _pageTitle = await _privacy.GetPageTitleAsync();
        Assert.That(_pageTitle, Is.EqualTo(expectedTitle),
            $"Expected page title '{expectedTitle}' but got '{_pageTitle}'");
    }

    /// <summary>
    /// Then the page content should be "{expectedContent}"
    /// 
    /// Extracts the paragraph text and verifies it matches the expected content
    /// passed from the feature file. This ensures the Privacy page has not been
    /// accidentally modified or broken.
    /// </summary>
    [Then("the page content should be \"(.*)\"")]
    public async Task ThenThePageContentShouldBeAsync(string expectedContent)
    {
        _pageContent = await _privacy.GetContentAsync();
        Assert.That(_pageContent, Is.Not.Empty, "Privacy page should have content");
        Assert.That(_pageContent, Is.EqualTo(expectedContent),
            $"Expected exact content but got '{_pageContent}'");
    }
}
