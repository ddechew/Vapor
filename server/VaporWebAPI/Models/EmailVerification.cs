namespace VaporWebAPI.Models;

/// <summary>
/// Represents an email verification token associated with a user account.
/// Used to verify a user's email address upon registration or email change.
/// </summary>
public partial class EmailVerification
{
    /// <summary>
    /// Primary key of the email verification record.
    /// </summary>
    public int VerificationId { get; set; }

    /// <summary>
    /// Foreign key referencing the user associated with this verification token.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Unique token string used for email verification.
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// The expiration date and time of the verification token.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// The date and time when the verification token was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp of the last modification (suffix required: 21180128).
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the associated user.
    /// </summary>
    public virtual User User { get; set; } = null!;
}
