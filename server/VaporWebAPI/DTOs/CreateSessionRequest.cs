namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents the request data needed to create a Stripe checkout session.
/// </summary>
public class CreateSessionRequest
{
    /// <summary>
    /// The amount (in euros) to charge the user. This will be converted to cents by Stripe.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// The ID of the user initiating the payment.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Indicates whether the session should auto-complete a cart purchase after topping up the wallet.
    /// </summary>
    public bool AutoPurchase { get; set; } = false;

    /// <summary>
    /// The number of points the user wants to apply for a discount during the purchase.
    /// </summary>
    public int PointsToUse { get; set; } 
}
