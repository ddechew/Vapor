namespace VaporWebAPI.Utils;

/// <summary>
/// Configuration settings required for integrating with the Stripe API.
/// These values are typically injected via the application's configuration (e.g., appsettings.json).
/// </summary>
public class StripeSettings
{
    /// <summary>
    /// The secret API key used to authenticate server-side requests to Stripe.
    /// </summary>
    public string SecretKey { get; set; }

    /// <summary>
    /// The public key used on the client side for Stripe Checkout or Elements.
    /// </summary>
    public string PublishableKey { get; set; }

    /// <summary>
    /// The webhook secret used to validate incoming events from Stripe webhooks.
    /// </summary>
    public string WebhookSecret { get; set; }
}
