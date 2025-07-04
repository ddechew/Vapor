namespace VaporWebAPI.Models;

/// <summary>
/// Represents a password reset token issued to a user for password recovery purposes.
/// </summary>
public partial class PasswordResetToken
{
    /// <summary>
    /// Primary key of the password reset token record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The ID of the user this token belongs to.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The token string used to authenticate the password reset request.
    /// </summary>
    public string Token { get; set; } = null!;

    /// <summary>
    /// The expiration time of the token after which it becomes invalid.
    /// </summary>
    public DateTime ExpirationTime { get; set; }

    /// <summary>
    /// Timestamp of the last modification for audit purposes.
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the user this token belongs to.
    /// </summary>
    public virtual User User { get; set; } = null!;
}
