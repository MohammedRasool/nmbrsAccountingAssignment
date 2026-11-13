using Reqnroll.BoDi;
using NmbrsAccounting.Tests.Support;
using Reqnroll;

namespace NmbrsAccounting.Tests.Hooks;

/// <summary>
/// Reqnroll lifecycle hooks that wire Playwright into the test execution.
/// 
/// FLOW:
///   1. [BeforeScenario] - Creates a fresh PlaywrightDriver (browser context + page)
///   2. [Step Execution] - Steps use the injected driver via DI
///   3. [AfterScenario]  - Takes screenshot on failure, then closes the context
///   4. [AfterTestRun]   - Shuts down the browser entirely
/// 
/// The driver is registered in Reqnroll's DI container, so step definitions
/// and page objects receive it via constructor injection automatically.
/// </summary>
[Binding]
public sealed class PlaywrightHooks
{
    private readonly IObjectContainer _container;   // Reqnroll's DI container
    private readonly ScenarioContext _scenario;     // Tracks scenario state (pass/fail)

    public PlaywrightHooks(IObjectContainer container, ScenarioContext scenario)
    {
        _container = container;
        _scenario = scenario;
    }

    /// <summary>
    /// Runs BEFORE each scenario. Creates a new PlaywrightDriver with a fresh
    /// browser context (incognito) and registers it for dependency injection.
    /// </summary>
    [BeforeScenario]
    public async Task BeforeScenarioAsync()
    {
        var driver = new PlaywrightDriver();
        await driver.StartScenarioAsync();  // Creates new context + page
        _container.RegisterInstanceAs(driver); // Now injectable into steps & pages
    }

    /// <summary>
    /// Runs AFTER each scenario (even if it failed). Captures a screenshot
    /// on failure, then closes the browser context to free memory.
    /// </summary>
    [AfterScenario]
    public async Task AfterScenarioAsync()
    {
        var driver = _container.Resolve<PlaywrightDriver>();

        // If the test failed, take a screenshot for debugging
        if (_scenario.ScenarioExecutionStatus != ScenarioExecutionStatus.OK)
            await driver.CaptureScreenshotOnFailureAsync(_scenario.ScenarioInfo.Title);

        // Close the incognito context (destroys tab + cookies + cache)
        await driver.StopScenarioAsync();
    }

    /// <summary>
    /// Runs ONCE after all scenarios complete. Shuts down the browser process.
    /// </summary>
    [AfterTestRun]
    public static Task AfterTestRunAsync() => PlaywrightDriver.ShutdownAsync();
}