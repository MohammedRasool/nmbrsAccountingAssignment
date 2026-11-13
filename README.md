# Nmbrs Accounting - End-to-End QA Automation Suite

Automated E2E test suite for the NmbrsAccounting Web Application, built with C#, Playwright, and Reqnroll (BDD).

### Author: [Abdul Azeez Mohammed Rasool](https://www.linkedin.com/in/abdul-azeez-mohammed-rasool/)
#### Email: razuldt@gmail.com

---

## Table of Contents

- [Technologies Used](#technologies-used)
- [Clone & Setup](#clone--setup)
- [Project Structure](#project-structure)
- [Architecture & Design Patterns](#architecture--design-patterns)
- [Test Coverage](#test-coverage)
- [Configuration](#configuration)
- [Reporting](#reporting)
- [Troubleshooting](#troubleshooting)

---

## Technologies Used

| Technology | Purpose |
| :--- | :--- |
| C# / .NET 10.0 | Primary language and runtime |
| Playwright | Browser automation |
| Reqnroll | BDD framework (Gherkin syntax) |
| NUnit | Test runner and assertions |
| GitHub Actions | CI/CD pipeline |

---

## Clone & Setup

### 1. Clone the Repository

```bash
git clone https://github.com/MohammedRasool/nmbrsAccountingAssignment.git
cd nmbrsAccountingAssignment
```

### 2. Start the Web Application

Open a terminal and run:

```bash
cd nmbrsaccounting_webapp/NmbrsAccounting.WebApp
dotnet run
```

The app will be available at `https://localhost:5001`.

### 3. Install Playwright Browsers

In a new terminal, from the repository root:

```bash
cd nmbrsaccounting_testsuite/NmbrsAccounting.Tests
dotnet build
pwsh bin/Debug/net10.0/playwright.ps1 install chromium
```

> **Note:** If `pwsh` is not available, use `powershell` on Windows or install PowerShell Core on macOS/Linux.

### 4. Run the Tests

Ensure the web app is still running, then:

```bash
cd nmbrsaccounting_testsuite/NmbrsAccounting.Tests
dotnet test
```

---

## Project Structure

```
nmbrsaccounting-qa-assessment/
├── .github/
│   └── workflows/
│       └── e2e-tests.yml          # GitHub Actions CI/CD pipeline
├── nmbrsaccounting_testsuite/
│   └── NmbrsAccounting.Tests/
│       ├── Features/
│       │   ├── Navigation.feature  # Menu navigation scenarios
│       │   └── Invoices.feature    # Invoice verification scenarios
│       ├── Hooks/
│       │   └── PlaywrightHooks.cs  # Before/After scenario lifecycle
│       ├── Pages/
│       │   ├── BasePage.cs         # Shared navigation logic
│       │   ├── HomePage.cs         # Home page object
│       │   └── InvoicesPage.cs     # Invoices page object
│       ├── StepDefinitions/
│       │   ├── NavigationSteps.cs  # Navigation step bindings
│       │   └── InvoiceSteps.cs     # Invoice step bindings
│       ├── Support/
│       │   ├── PlaywrightDriver.cs # Browser lifecycle management
│       │   └── Money.cs            # Currency parsing utility
│       ├── AssemblyInfo.cs         # Parallel execution config
│       ├── NmbrsAccounting.Tests.csproj
│       └── reqnroll.json           # Reqnroll configuration
├── nmbrsaccounting_webapp/         # ASP.NET Core web app under test
├── .gitignore
└── README.md
```

---

## Architecture & Design Patterns

### Page Object Model (POM)

Web elements and interactions are encapsulated in page objects (`Pages/`). If the UI changes, updates are localized to a single file.

**Inheritance:** `BasePage` centralizes shared logic (menu navigation, load-state verification) that all page objects inherit.

### Dependency Injection

The `PlaywrightDriver` (browser lifecycle) is injected into steps and pages via Reqnroll's `IObjectContainer`, eliminating redundant setup code.

### Lifecycle Hooks

`PlaywrightHooks.cs` manages the browser lifecycle:

| Hook | Purpose |
| :--- | :--- |
| `[BeforeScenario]` | Create fresh browser context (incognito), register driver in DI |
| `[AfterScenario]` | Capture screenshot on failure, close browser context |
| `[AfterTestRun]` | Shut down browser process |

### Dynamic Navigation

The `Navigation.feature` scenario reads menu items directly from the DOM at runtime. If a developer adds a new page to the menu, the test automatically discovers and verifies it without code changes.

### Performance Optimizations

- **Parallel Execution:** `[Parallelizable(ParallelScope.Fixtures)]` runs feature files concurrently
- **Singleton Browser:** One browser instance shared across all tests (fast startup)
- **Isolated Contexts:** Each scenario gets its own `BrowserContext` (no state leakage)
- **Headless by Default:** Suitable for CI/CD agents; set `HEADED=true` for debugging
- **No Hardcoded Waits:** Uses Playwright's auto-waiting and explicit `WaitForLoadState` calls

### Structure-Based Selectors

Locators use CSS classes and ARIA roles (e.g., `.summary-row`, `main[role='main']`) rather than fragile text matches or XPaths.

---

## Test Coverage

| Feature | Scenario | Validation Logic |
| :--- | :--- | :--- |
| **Navigation** | Every page in the menu opens successfully | Dynamically reads menu links, clicks them, asserts HTTP 200 + visible main content |
| **Invoices** | Summary row contains correct total | Sums all invoice amounts and asserts it matches the UI's summary row |
| **Invoices** | Invoice I634 shows expected amount | Extracts amount for invoice `I634` and asserts it equals `423.99 EUR` |

---

## Configuration

Environment variables for adapting to different environments:

| Variable | Default | Description |
| :--- | :--- | :--- |
| `BASE_URL` | `https://localhost:5001` | Target URL of the web application |
| `HEADED` | `false` | Set to `true` for visible browser window (debugging) |

### Browser Support

Currently runs on **Chromium only**. To add Firefox or WebKit, modify the `LaunchAsync` call in `PlaywrightDriver.cs`.

---

## Reporting

| Artifact | Description |
| :--- | :--- |
| **Reqnroll HTML Report** | Human-readable report at `nmbrsaccounting_testsuite/NmbrsAccounting.Tests/bin/Debug/net10.0/ReqnrollExecutionReport.html` |
| **TRX Format** | Machine-readable results via `--logger trx` (consumed by Azure DevOps / GitHub Actions) |
| **Failure Screenshots** | Full-page screenshots saved to `artifacts/` on scenario failure |

---

## Troubleshooting

| Issue | Solution |
| :--- | :--- |
| `Browser not found` / `Executable doesn't exist` | Run: `pwsh bin/Debug/net10.0/playwright.ps1 install chromium` |
| `Connection refused` on `https://localhost:5001` | Ensure web app is running (`dotnet run` in WebApp folder) |
| Tests timeout | Increase timeouts in `PlaywrightDriver.cs` or check web app responsiveness |
| `pwsh` command not found | Use `powershell` on Windows, or install PowerShell Core: `brew install powershell` |
