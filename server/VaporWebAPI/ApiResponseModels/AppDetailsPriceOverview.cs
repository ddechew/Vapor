namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the price overview section of the Steam app details API response.
/// </summary>
public class AppDetailsPriceOverview
{
    /// <summary>
    /// The final price of the app in minor currency units (e.g., cents).
    /// </summary>
    [JsonPropertyName("final")]
    public int FinalPrice { get; set; }

    /// <summary>
    /// The currency code (e.g., "USD", "EUR") for the price.
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; }
}
