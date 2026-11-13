using NmbrsAccounting.Tests.Pages;
using NUnit.Framework;
using Reqnroll;

namespace NmbrsAccounting.Tests.StepDefinitions;

/// <summary>
/// Step definitions for the Navigation.feature file.
/// Binds Gherkin steps to code that interacts with the HomePage object.
/// 
/// FLOW:
///   Given I am on the home page          → Navigate to base URL
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

    /// <summary>Given I am on the home page</summary>
    [Given("I am on the home page")]
    public Task GivenIAmOnTheHomePageAsync() => _home.OpenAsync();

    /// <summary>Given I am on the "{page}" page (e.g., "Invoices", "Privacy")</summary>
    [Given(@"I am on the ""(.*)"" page")]
    public Task GivenIAmOnThePageAsync(string label) => _home.OpenMenuPageAsync(label);

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