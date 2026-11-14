using NmbrsAccounting.Tests.Pages;
using NUnit.Framework;
using Reqnroll;

namespace NmbrsAccounting.Tests.StepDefinitions;

/// <summary>
/// Step definitions for the Navigation.feature file.
/// Binds Gherkin steps to code that interacts with the HomePage object.
/// 
/// FLOW:
///   Given I am on the "Home" page        → Navigate to Home page via menu
///   When I visit every page in the menu  → Loop through menu links, click each one
///   Then every visited page loads        → Assert no pages returned errors
/// </summary>
[Binding]
public sealed class NavigationSteps
{
    private readonly HomePage _home;           // Page object injected via DI
    private readonly List<string> _visited = new();  // Track which pages we visited
    private readonly List<string> _broken = new();   // Track pages that failed to load

    // Constructor receives HomePage via Reqnroll's dependency injection
    public NavigationSteps(HomePage home) => _home = home;

    /// <summary>Given I am on the "{page}" page (e.g., "Home", "Invoices", "Privacy")</summary>
    [Given(@"I am on the ""(.*)"" page")]
    public Task GivenIAmOnThePageAsync(string label) => _home.OpenMenuPageAsync(label);

    /// <summary>
    /// Then the page title should be "{expectedTitle}"
    /// 
    /// Extracts the h1 element text from the current page and verifies it matches
    /// the expected title. This ensures each page displays the correct heading.
    /// </summary>
    [Then(@"the page title should be ""(.*)""")]
    public async Task ThenThePageTitleShouldBeAsync(string expectedTitle)
    {
        var actualTitle = await _home.GetPageTitleAsync();
        Assert.That(actualTitle, Is.EqualTo(expectedTitle),
            $"Expected page title '{expectedTitle}' but got '{actualTitle}'");
    }

    /// <summary>
    /// When I visit every page listed in the menu
    /// 
    /// Dynamically reads menu labels from the DOM, clicks each link,
    /// and tracks any pages that fail to load (HTTP error or missing content).
    /// </summary>
    [When("I visit every page listed in the menu")]
    public async Task WhenIVisitEveryPageListedInTheMenuAsync()
    {
        foreach (var label in await _home.GetMenuLabelsAsync())
        {
            _visited.Add(label);
            var response = await _home.OpenMenuPageAsync(label);  // Click and get HTTP response
            
            // Check if the page loaded successfully (HTTP 200 + main content visible)
            if (!response.Ok || !await _home.MainContentIsVisibleAsync())
                _broken.Add($"{label} (HTTP {(int)response.Status})");
        }
    }

    /// <summary>
    /// Then every visited page loads successfully
    /// 
    /// Asserts that we visited at least one page and none of them failed.
    /// </summary>
    [Then("every visited page loads successfully")]
    public void ThenEveryVisitedPageLoadsSuccessfully()
    {
        Assert.That(_visited, Is.Not.Empty, "the menu should contain at least one page");
        Assert.That(_broken, Is.Empty, $"broken pages: {string.Join(", ", _broken)}");
    }
}