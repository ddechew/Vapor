namespace VaporWebAPI.Models;

/// <summary>
/// Represents a payment token used to track Stripe checkout sessions and associated wallet payments.
/// </summary>
public partial class PaymentToken
{
    /// <summary>
    /// Primary key of the payment token record.
    /// </summary>
    public int PaymentTokenId { get; set; }

    /// <summary>
    /// The Stripe checkout session ID linked to this payment.
    /// </summary>
    public string SessionId { get; set; } = null!;

    /// <summary>
    /// The ID of the user who initiated the payment.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The monetary amount involved in the payment.
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// A unique token representing the payment operation.
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// Timestamp when the payment token was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp when the payment token expires and becomes invalid.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Timestamp of the last modification for audit purposes.
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the user associated with this payment token.
    /// </summary>
    public virtual User User { get; set; } = null!;
}
