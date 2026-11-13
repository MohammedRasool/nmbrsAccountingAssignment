# Nmbrs Accounting - End-to-End QA Automation Suite

This project contains an automated End-to-End (E2E) test suite for the **NmbrsAccounting Web Application**. It is built using **C#**, **Playwright**, and **Reqnroll** (BDD), designed to be robust, maintainable, and ready for CI/CD pipelines.

### Author: [Abdul Azeez Mohammed Rasool](https://www.linkedin.com/in/abdul-azeez-mohammed-rasool/)
#### Email: razuldt@gmail.com

## 🚀 Quick Start

### Prerequisites
* .NET SDK 10.0 or higher
* A running instance of the `NmbrsAccounting.WebApp`

### 1. Start the Web Application
From the repository root, open a terminal and run:
```bash
cd nmbrsaccounting_webapp/NmbrsAccounting.WebApp
dotnet run
```
*(The app will be available at `https://localhost:5001`)*

### 2. Install Playwright Browsers
If running for the first time, download the required browser binaries:
```bash
cd nmbrsaccounting_testsuite/NmbrsAccounting.Tests
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```
> **Note:** If `pwsh` is not available, use PowerShell (`powershell`) on Windows or install PowerShell Core on macOS/Linux.

### 3. Run the Tests
Execute the test suite (ensure the web app is still running):
```bash
cd nmbrsaccounting_testsuite/NmbrsAccounting.Tests
dotnet test
```

### 4. View Reports
* **HTML Report:** After running tests, open `nmbrsaccounting_testsuite/NmbrsAccounting.Tests/bin/Debug/net10.0/ReqnrollExecutionReport.html` in your browser.
* **Failure Screenshots:** If a test fails, a full-page screenshot is automatically saved to the `artifacts/` folder (relative to the test project directory).

---

## 🏗️ Architecture & Design Decisions

This solution was designed to strictly adhere to the assignment's requirements regarding best practices, performance, and unattended execution.

### 1. Reusability & Code Structure (Addressing "Opportunities to reuse code")
* **Page Object Model (POM):** Web elements and interactions are encapsulated in `Pages/` (e.g., `InvoicesPage.cs`). This ensures that if the UI changes, updates are localized to a single file.
* **Inheritance (`BasePage`):** Shared logic, such as menu navigation and load-state verification, is centralized in `BasePage.cs` and inherited by all specific page objects.
* **Dependency Injection:** The `PlaywrightDriver` (browser lifecycle) is injected into steps and pages via Reqnroll's `IObjectContainer`, eliminating redundant setup code.
* **Utility Classes:** Complex logic, like parsing currency strings (`423.99 EUR`) into decimals, is isolated in the `Support/Money.cs` helper.

### 2. Resilience to Change (Addressing "Pages can be added/changed")
* **Dynamic Navigation:** Instead of hardcoding page names, the `Navigation.feature` scenario reads the menu items directly from the DOM at runtime. If a developer adds a "Clients" page to the menu next week, the test will automatically discover, click, and verify it without requiring any code changes.
* **Structure-Based Selectors:** Locators rely on CSS classes and ARIA roles (e.g., `.summary-row`, `main[role='main']`) rather than fragile text matches or absolute XPaths.

### 3. Performance & CI/CD Readiness (Addressing "Unattended contexts")
* **Parallel Execution:** The suite is configured with `[Parallelizable(ParallelScope.Fixtures)]` to run feature files concurrently, drastically reducing pipeline execution time.
* **Optimized Browser Lifecycle:** A single browser instance is shared across the entire test run (saving memory and startup time), while each scenario gets its own isolated `BrowserContext` (ensuring test independence and preventing state leakage).
* **Headless by Default:** Tests run in headless mode unless the `HEADED=true` environment variable is set, making them perfectly suited for CI/CD agents.
* **No Hardcoded Waits:** The suite relies entirely on Playwright's auto-waiting mechanisms and explicit `WaitForLoadState` calls, avoiding brittle `Thread.Sleep()` delays.

---

##  Test Coverage

The suite implements the scenarios suggested by the web application:

| Feature | Scenario | Validation Logic |
| :--- | :--- | :--- |
| **Navigation** | Every page in the menu opens successfully | Dynamically reads menu links, clicks them, and asserts HTTP 200 + visible main content. |
| **Invoices** | Summary row contains correct total | Sums all individual invoice amounts (using `decimal` precision) and asserts it matches the UI's summary row. |
| **Invoices** | Invoice I634 shows expected amount | Extracts the amount for invoice `I634` and asserts it equals `423.99 EUR`. |

---

## ⚙️ Configuration

The framework is highly configurable via Environment Variables, making it easy to adapt to different environments (Local, QA, Staging):

| Variable | Default | Description |
| :--- | :--- | :--- |
| `BASE_URL` | `https://localhost:5001` | The target URL of the web application. |
| `HEADED` | `false` | Set to `true` to run tests with a visible browser window (useful for debugging). |

### Browser Support

This suite currently runs on **Chromium only** (configured in `PlaywrightDriver.cs` via `_playwright.Chromium.LaunchAsync()`). This is sufficient for the assignment's scope. To add Firefox or WebKit, modify the `LaunchAsync` call or add additional browser parameters.

---

## 📊 Reporting

To ensure transparency in unattended pipelines, the suite generates multiple reporting artifacts:
1. **Reqnroll HTML Report:** A human-readable living document generated via `reqnroll.json`.
2. **TRX Format:** Machine-readable results generated via `--logger trx`, easily consumed by Azure DevOps or GitHub Actions.
3. **Diagnostics:** Automatic full-page screenshots saved to `artifacts/` upon scenario failure.

---

## 🔧 Troubleshooting

| Issue | Solution |
| :--- | :--- |
| `Browser not found` / `Executable doesn't exist` | Run the Playwright install command again: `pwsh bin/Debug/net10.0/playwright.ps1 install chromium` |
| `Connection refused` on `https://localhost:5001` | Ensure the web app is running (`dotnet run` in the WebApp folder). Check that port 5001 is not blocked. |