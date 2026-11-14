using System.Globalization;

namespace NmbrsAccounting.Tests.Support;

/// <summary>
/// Utility class for parsing currency strings into decimal values.
/// 
/// Handles the EUR format used in the invoices page:
///   - "499.99 EUR"  → 499.99
///   - "423.99 EUR"  → 423.99
///   - "39.99 EUR"   → 39.99
/// 
/// PARSING LOGIC:
///   1. Remove "EUR" currency code
///   2. Trim whitespace
///   3. Parse with CultureInfo.InvariantCulture (dot as decimal separator)
/// </summary>
public static class Money
{
    /// <summary>
    /// Parses a currency string in EUR format (e.g., "499.99 EUR") into a decimal value.
    /// </summary>
    public static decimal Parse(string raw)
    {
        // Remove "EUR" and trim whitespace
        var cleaned = raw.Replace("EUR", "").Trim();
        
        // Parse using invariant culture (expects dot as decimal separator)
        return decimal.Parse(cleaned, CultureInfo.InvariantCulture);
    }
}