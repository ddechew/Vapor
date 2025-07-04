namespace VaporWebAPI.Models;

/// <summary>
/// Represents a token used to authorize an email change request for a user.
/// </summary>
public partial class EmailChangeToken
{
    /// <summary>
    /// Primary key for the email change token record.
    /// </summary>
    public int EmailChangeTokenId { get; set; }

    /// <summary>
    /// Foreign key referencing the user who requested the email change.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The new email address requested by the user.
    /// </summary>
    public string NewEmail { get; set; } = null!;

    /// <summary>
    /// Unique token string used for verifying the email change.
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// The expiration date and time of this token.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// The date and time when this token was created.
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
