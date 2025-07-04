namespace VaporWebAPI.Models;

/// <summary>
/// Represents a user of the Vapor platform.
/// Contains authentication details, profile information, wallet, points, and relations to other entities.
/// </summary>
public partial class User
{
    /// <summary>
    /// Primary key identifier for the user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// External Google account identifier, if linked.
    /// </summary>
    public string? GoogleId { get; set; }

    /// <summary>
    /// Foreign key to the user's role.
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Unique username used for login and display.
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// Display name shown publicly; can be different from username.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// Hashed password for authentication.
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// URL path to the user's profile picture.
    /// </summary>
    public string? ProfilePicture { get; set; }

    /// <summary>
    /// User's email address.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Discount points accumulated by the user, can be redeemed for discounts.
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// Wallet balance in currency.
    /// </summary>
    public decimal Wallet { get; set; }

    /// <summary>
    /// Refresh token used for JWT token renewal.
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Expiry date and time of the refresh token.
    /// </summary>
    public DateTime? RefreshTokenExpiryTime { get; set; }

    /// <summary>
    /// Flag indicating whether the user's email has been verified.
    /// </summary>
    public bool IsEmailVerified { get; set; }

    /// <summary>
    /// Flag indicating whether the user logged in via Google OAuth.
    /// </summary>
    public bool IsGoogleAuthenticated { get; set; }

    /// <summary>
    /// Timestamp of the last modification to the user record.
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Apps owned by the user.
    /// </summary>
    public virtual ICollection<AppLibrary> AppLibraries { get; set; } = new List<AppLibrary>();

    /// <summary>
    /// Reviews submitted by the user.
    /// </summary>
    public virtual ICollection<AppReview> AppReviews { get; set; } = new List<AppReview>();

    /// <summary>
    /// Items currently in the user's cart.
    /// </summary>
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    /// <summary>
    /// Pending email change tokens issued to the user.
    /// </summary>
    public virtual ICollection<EmailChangeToken> EmailChangeTokens { get; set; } = new List<EmailChangeToken>();

    /// <summary>
    /// Email verification tokens issued to the user.
    /// </summary>
    public virtual ICollection<EmailVerification> EmailVerifications { get; set; } = new List<EmailVerification>();

    /// <summary>
    /// Notifications received by the user.
    /// </summary>
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    /// <summary>
    /// Password reset tokens issued to the user.
    /// </summary>
    public virtual ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();

    /// <summary>
    /// Payment tokens associated with the user (e.g., Stripe sessions).
    /// </summary>
    public virtual ICollection<PaymentToken> PaymentTokens { get; set; } = new List<PaymentToken>();

    /// <summary>
    /// Comments made by the user on posts.
    /// </summary>
    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    /// <summary>
    /// Likes the user has given to posts.
    /// </summary>
    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    /// <summary>
    /// Posts created by the user.
    /// </summary>
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    /// <summary>
    /// Purchase history records of the user.
    /// </summary>
    public virtual ICollection<PurchaseHistory> PurchaseHistories { get; set; } = new List<PurchaseHistory>();

    /// <summary>
    /// The role entity associated with the user.
    /// </summary>
    public virtual Role Role { get; set; } = null!;

    /// <summary>
    /// Wishlist items saved by the user.
    /// </summary>
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
