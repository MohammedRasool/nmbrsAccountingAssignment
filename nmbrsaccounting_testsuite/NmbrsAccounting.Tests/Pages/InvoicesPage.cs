using NmbrsAccounting.Tests.Support;

namespace NmbrsAccounting.Tests.Pages;

/// <summary>
/// Represents a single row in the invoices table.
/// Using a record for immutability and value-based equality.
/// </summary>
public sealed record InvoiceRow(string Number, decimal Amount);

/// <summary>
/// Page object for the Invoices page. Extracts invoice data from the HTML table.
/// 
/// TABLE STRUCTURE (from Invoices.cshtml):
///   table
///     thead: [Invoice Number | Client | Amount]
///     tbody:
///       tr: [I523 | Microsoft | 499.99 EUR]
///       tr: [I634 | Amazon    | 423.99 EUR]
///       tr: [I125 | Slack     |  39.99 EUR]
///       tr.summary-row: [Sum of invoices || 963.97 EUR]
/// 
/// SELECTORS:
///   - Data rows: table tbody tr:not(.summary-row) - excludes the summary row
///   - Summary row: table tbody tr.summary-row
///   - Amount cells: td.number-cell - the right-aligned currency column
///   - Number cells: td:first-child - the invoice number column
/// </summary>
public sealed class InvoicesPage : BasePage
{
    // CSS selectors matching the table structure in Invoices.cshtml
    private const string DataRowSelector = "table tbody tr:not(.summary-row)";  // All rows except summary
    private const string SummaryRowSelector = "table tbody tr.summary-row";     // The total row
    private const string AmountCellSelector = "td.number-cell";                 // Currency column
    private const string NumberCellSelector = "td:first-child";                 // Invoice number column

    // Constructor receives PlaywrightDriver via Reqnroll's dependency injection
    public InvoicesPage(PlaywrightDriver driver) : base(driver) { }

    /// <summary>
    /// Extracts all invoice rows from the table.
    /// 
    /// FLOW:
    ///   1. Find all data rows (excluding summary)
    ///   2. For each row, extract the invoice number and amount
    ///   3. Parse the amount string (e.g. "423.99 EUR") into a decimal
    ///   4. Return as a list of InvoiceRow records
    /// </summary>
    public async Task<IReadOnlyList<InvoiceRow>> GetInvoiceRowsAsync()
    {
        var rows = Page.Locator(DataRowSelector);
        var count = await rows.CountAsync();
        var result = new List<InvoiceRow>(count);

        for (var i = 0; i < count; i++)
        {
            var row = rows.Nth(i);  // Access the i-th row
            
            // Extract invoice number from the first column
            var number = (await row.Locator(NumberCellSelector).InnerTextAsync()).Trim();
            
            // Extract amount from the number-cell column and parse it
            var amount = Money.Parse(await row.Locator(AmountCellSelector).InnerTextAsync());
            
            result.Add(new InvoiceRow(number, amount));
        }

        return result;
    }

    /// <summary>
    /// Extracts the total amount from the summary row.
    /// The summary row has class "summary-row" and contains the total in a number-cell.
    /// </summary>
    public async Task<decimal> GetSummaryTotalAsync()
        => Money.Parse(await Page.Locator(SummaryRowSelector)
            .Locator(AmountCellSelector).InnerTextAsync());

    /// <summary>
    /// Finds a specific invoice by its number and returns its amount.
    /// Throws InvalidOperationException if the invoice is not found.
    /// </summary>
    public async Task<decimal> GetAmountForInvoiceAsync(string invoiceNumber)
    {
        // Get all rows and find the one matching the invoice number (case-insensitive)
        var row = (await GetInvoiceRowsAsync())
            .FirstOrDefault(r => r.Number.Equals(invoiceNumber, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Invoice '{invoiceNumber}' was not found on the Invoices page.");

        return row.Amount;
    }
}