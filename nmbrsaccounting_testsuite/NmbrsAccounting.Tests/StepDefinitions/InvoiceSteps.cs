using NmbrsAccounting.Tests.Pages;
using NmbrsAccounting.Tests.Support;
using NUnit.Framework;
using Reqnroll;

namespace NmbrsAccounting.Tests.StepDefinitions;

/// <summary>
/// Step definitions for the Invoices.feature file.
/// Binds Gherkin steps to code that interacts with the InvoicesPage object.
/// 
/// SCENARIO 1: Verify summary row total
///   When I read all invoice amounts   → Extract all rows from the table
///   Then the summary equals the sum   → Sum the amounts, compare to summary row
/// 
/// SCENARIO 2: Verify specific invoice amount
///   When I read the amount of "I634"  → Find the row, extract the amount
///   Then the amount equals "423.99"   → Compare to expected value
/// </summary>
[Binding]
public sealed class InvoiceSteps
{
    private readonly InvoicesPage _invoices;                    // Page object injected via DI
    private IReadOnlyList<InvoiceRow> _rows = Array.Empty<InvoiceRow>();  // Stores extracted rows
    private decimal _actualAmount;                              // Stores amount for single-invoice check

    // Constructor receives InvoicesPage via Reqnroll's dependency injection
    public InvoiceSteps(InvoicesPage invoices) => _invoices = invoices;

    /// <summary>
    /// When I read all invoice amounts from the overview
    /// 
    /// Extracts all invoice rows (number + amount) from the table.
    /// Stores them in _rows for the next step to use.
    /// </summary>
    [When("I read all invoice amounts from the overview")]
    public async Task WhenIReadAllInvoiceAmountsFromTheOverviewAsync()
        => _rows = await _invoices.GetInvoiceRowsAsync();

    /// <summary>
    /// Then the summary row total equals the sum of those amounts
    /// 
    /// Sums all extracted invoice amounts and compares to the summary row.
    /// Example: 499.99 + 423.99 + 39.99 = 963.97 (must match summary row)
    /// </summary>
    [Then("the summary row total equals the sum of those amounts")]
    public async Task ThenTheSummaryRowTotalEqualsTheSumOfThoseAmountsAsync()
    {
        Assert.That(_rows, Is.Not.Empty, "expected at least one invoice row");

        var sum = _rows.Sum(r => r.Amount);  // Calculate sum of all invoice amounts
        var summary = await _invoices.GetSummaryTotalAsync();  // Read the summary row

        Assert.That(summary, Is.EqualTo(sum),
            $"summary row shows {summary} but the invoices sum to {sum}");
    }

    /// <summary>
    /// When I read the amount of invoice "{invoiceNumber}"
    /// 
    /// Finds the specific invoice row by number and extracts its amount.
    /// Stores it in _actualAmount for the next step to verify.
    /// </summary>
    [When(@"I read the amount of invoice ""(.*)""")]
    public async Task WhenIReadTheAmountOfInvoiceAsync(string invoiceNumber)
        => _actualAmount = await _invoices.GetAmountForInvoiceAsync(invoiceNumber);

    /// <summary>
    /// Then the amount equals "{expectedAmount}"
    /// 
    /// Parses the expected amount string (e.g., "423.99 EUR") and compares
    /// to the actual amount extracted in the previous step.
    /// </summary>
    [Then(@"the amount equals ""(.*)""")]
    public void ThenTheAmountEquals(string expectedAmount)
        => Assert.That(_actualAmount, Is.EqualTo(Money.Parse(expectedAmount)),
            $"expected {expectedAmount} but the page showed {_actualAmount}");
}