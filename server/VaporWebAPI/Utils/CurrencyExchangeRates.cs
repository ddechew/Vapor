namespace VaporWebAPI.Utils;

/// <summary>
/// Provides currency exchange rate utilities for converting to Euro.
/// </summary>
public static class CurrencyExchangeRates
{
    /// <summary>
    /// A dictionary mapping currency codes to their exchange rate relative to the Euro.
    /// </summary>
    public static readonly Dictionary<string, decimal> ExchangeRatesToEuro = new()
    {
        { "USD", 0.92m },
        { "GBP", 1.17m },
        { "JPY", 0.0062m },
        { "CNY", 0.13m },
        { "BGN", 0.51m },
        { "EUR", 1.0m },
        { "CAD", 0.6349m },
        { "CHF", 0.693m },
        { "SEK", 0.091m },
        { "NZD", 0.579m },
        { "HKD", 0.118m },
        { "SGD", 0.689m },
        { "MXN", 0.052m },
        { "INR", 0.011m },
        { "BRL", 0.056m },
        { "ZAR", 0.056m },
        { "RUB", 0.011m },
        { "COP", 0.00023m },
        { "CRC", 0.0016m },
        { "ILS", 0.24m },
        { "PLN", 0.23m },
        { "THB", 0.026m },
        { "VND", 0.000038m },
        { "CLP", 0.0010m },
        { "KZT", 0.0019m },
        { "NOK", 0.086m }, 
        { "QAR", 0.25m },
        { "SAR", 0.245m },
        { "KRW", 0.00069m },
        { "AED", 0.25m },
        { "AUD", 0.60m },
        { "IDR", 0.000059m },
        { "KWD", 2.99m },
        { "MYR", 0.20m },
        { "PEN", 0.24m },
        { "PHP", 0.016m },
        { "TWD", 0.028m },
        { "UAH", 0.024m },
        { "UYU", 0.023m },
    };

    /// <summary>
    /// Checks if the given currency code is supported for conversion.
    /// </summary>
    /// <param name="currencyCode">The 3-letter currency code (e.g., USD, EUR).</param>
    /// <returns>True if supported, otherwise false.</returns>
    public static bool IsSupportedCurrency(string currencyCode)
    {
        return ExchangeRatesToEuro.ContainsKey(currencyCode.ToUpper());
    }

    /// <summary>
    /// Converts an amount from a given currency to Euro using predefined exchange rates.
    /// </summary>
    /// <param name="amount">The amount in the original currency.</param>
    /// <param name="currencyCode">The currency code of the original currency.</param>
    /// <returns>The equivalent amount in Euros, rounded to two decimal places.</returns>
    /// <exception cref="ArgumentException">Thrown when the currency code is not supported.</exception>
    public static decimal ConvertToEuro(decimal amount, string currencyCode)
    {
        currencyCode = currencyCode.ToUpper();

        if (ExchangeRatesToEuro.ContainsKey(currencyCode))
            return Math.Round(amount * ExchangeRatesToEuro[currencyCode], 2);

        throw new ArgumentException($"Unsupported currency code: {currencyCode}");
    }
}
