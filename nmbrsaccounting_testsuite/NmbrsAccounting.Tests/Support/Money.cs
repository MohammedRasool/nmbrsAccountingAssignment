using System.Globalization;
using System.Text.RegularExpressions;

namespace NmbrsAccounting.Tests.Support;

/// <summary>
/// Utility class for parsing currency strings into decimal values.
/// 
/// Handles multiple formats:
///   - "423.99 EUR"      → 423.99  (US format with currency code)
///   - "1,234.56 EUR"    → 1234.56 (US format with thousands separator)
///   - "€ 1.234,56"      → 1234.56 (European format: dot for thousands, comma for decimals)
/// 
/// PARSING LOGIC:
///   1. Remove 3-letter currency codes (EUR, USD, etc.)
///   2. Remove currency symbols (€, $, etc.)
///   3. Detect format by comparing last comma vs last dot position
///   4. Normalize to invariant format (no thousands separator, dot for decimals)
///   5. Parse with CultureInfo.InvariantCulture
/// </summary>
public static class Money
{
    // Matches 3-letter uppercase currency codes like "EUR", "USD", "GBP"
    private static readonly Regex CurrencyCode = new(@"\b[A-Z]{3}\b", RegexOptions.Compiled);

    /// <summary>
    /// Parses a currency string into a decimal value.
    /// </summary>
    public static decimal Parse(string raw)
    {
        // Step 1: Remove currency codes (e.g., "423.99 EUR" → "423.99 ")
        var s = CurrencyCode.Replace(raw, "");
        
        // Step 2: Remove everything except digits, dots, commas, and minus signs
        s = Regex.Replace(s, @"[^\d.,\-]", "");

        // Step 3: Detect format by comparing positions of last comma and last dot
        // European: "1.234,56" → last comma (5) > last dot (1) → remove dots, replace comma with dot
        // US:       "1,234.56" → last dot (5) > last comma (1) → just remove commas
        if (s.LastIndexOf(',') > s.LastIndexOf('.'))   // e.g. 1.234,56 (European)
            s = s.Replace(".", "").Replace(',', '.');
        else                                           // e.g. 1,234.56 or 423.99 (US)
            s = s.Replace(",", "");

        // Step 4: Parse using invariant culture (expects dot as decimal separator)
        return decimal.Parse(s, CultureInfo.InvariantCulture);
    }
}