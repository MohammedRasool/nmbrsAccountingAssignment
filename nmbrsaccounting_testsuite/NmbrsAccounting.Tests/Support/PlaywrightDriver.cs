using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace NmbrsAccounting.Tests.Support;

/// <summary>
/// Owns the Playwright lifecycle: one browser per test run (fast),
/// one isolated context + page per scenario (reliable, parallel-safe).
/// 
/// HIERARCHY:
///   IPlaywright → IBrowser (ONE per test run, expensive to start)
///                  └── IBrowserContext (ONE per scenario, isolated like incognito)
///                       └── IPage (the actual browser tab)
/// 
/// This design gives us fast startup (browser is reused) AND test isolation
/// (each scenario gets a clean context with no cookies/localStorage).
/// </summary>
public sealed class PlaywrightDriver
{
    // STATIC fields = shared across ALL test instances (singleton browser pattern)
    private static IPlaywright? _playwright;   // The Playwright process manager
    private static IBrowser? _browser;          // The single Chromium browser process
    private static readonly SemaphoreSlim BrowserLock = new(1, 1);  // Thread-safe lock for parallel test execution

    public IBrowserContext Context { get; private set; } = null!;
    public IPage Page { get; private set; } = null!;

    public static string BaseUrl =>
        Environment.GetEnvironmentVariable("BASE_URL") ?? "https://localhost:5001";

    private static bool Headed =>
        bool.TryParse(Environment.GetEnvironmentVariable("HEADED"), out var headed) && headed;

    /// <summary>
    /// Starts the browser if not already running. Uses double-checked locking
    /// to ensure only ONE browser is created even when tests run in parallel.
    /// </summary>
    public static async Task EnsureBrowserStartedAsync()
    {
        if (_browser is not null) return;  // Fast path: browser already running

        await BrowserLock.WaitAsync();     // Acquire lock (blocks other threads)
        try
        {
            if (_browser is not null) return;  // Check again after acquiring lock
            _playwright = await Playwright.CreateAsync();  // Start Playwright process manager
            _browser = await _playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions { Headless = !Headed }); // headless by default -> CI friendly
        }
        finally
        {
            BrowserLock.Release();  // Always release the lock
        }
    }

    /// <summary>
    /// Called ONCE after all tests complete. Closes the browser and disposes Playwright.
    /// This kills the Chromium OS process and frees memory.
    /// </summary>
    public static async Task ShutdownAsync()
    {
        if (_browser is not null) await _browser.CloseAsync();
        _playwright?.Dispose();
        _browser = null;
        _playwright = null;
    }

    /// <summary>
    /// Called BEFORE each scenario. Creates a fresh BrowserContext (incognito)
    /// and a new Page (tab). This ensures no state leaks between tests.
    /// </summary>
    public async Task StartScenarioAsync()
    {
        await EnsureBrowserStartedAsync();  // Ensure the shared browser is running

        // Create a NEW isolated context (like opening an incognito window)
        // Each scenario gets its own cookies, localStorage, cache - completely clean
        Context = await _browser!.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true, // self-signed localhost dev cert / unattended agents
        });
        
        // Create a new tab within this context
        Page = await Context.NewPageAsync();
        
        // Set timeouts to fail fast instead of hanging the CI pipeline
        Page.SetDefaultTimeout(10_000);           // 10s for actions (click, fill, etc.)
        Page.SetDefaultNavigationTimeout(15_000); // 15s for page loads
    }

    public Task OpenHomeAsync() => Page.GotoAsync(BaseUrl);

    /// <summary>
    /// Called AFTER each scenario. Closes the BrowserContext (not the browser).
    /// This destroys the tab, cookies, and all state - fast cleanup.
    /// The browser itself stays running for the next scenario.
    /// </summary>
    public async Task StopScenarioAsync() => await Context.CloseAsync();

    /// <summary>
    /// Captures a full-page screenshot when a test fails.
    /// Wrapped in try/catch so a screenshot failure never masks the original error.
    /// </summary>
    public async Task CaptureScreenshotOnFailureAsync(string scenarioTitle)
    {
        try
        {
            Directory.CreateDirectory("artifacts");
            var file = Path.Combine("artifacts",
                $"{Regex.Replace(scenarioTitle, @"[^a-zA-Z0-9]+", "_")}-{DateTime.UtcNow:HHmmss}.png");
            await Page.ScreenshotAsync(new PageScreenshotOptions { Path = file, FullPage = true });
            Console.WriteLine($"[artifact] {Path.GetFullPath(file)}");
        }
        catch
        {
            // never mask the original test failure
        }
    }
}