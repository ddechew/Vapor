namespace VaporWebAPI.Models;

/// <summary>
/// Represents a user-generated review for an app.
/// </summary>
public partial class AppReview
{
    /// <summary>
    /// Unique identifier for the review.
    /// </summary>
    public int ReviewId { get; set; }

    /// <summary>
    /// ID of the app that is being reviewed.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// ID of the user who wrote the review. Nullable for anonymous reviews.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// The display name of the user who wrote the review.
    /// </summary>
    public string UserDisplayName { get; set; } = null!;

    /// <summary>
    /// Indicates whether the user recommends the app.
    /// </summary>
    public bool IsRecommended { get; set; }

    /// <summary>
    /// The content of the review.
    /// </summary>
    public string ReviewText { get; set; } = null!;

    /// <summary>
    /// Timestamp when the review was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indicates if the review was edited after creation.
    /// </summary>
    public bool IsEdited { get; set; }

    /// <summary>
    /// Timestamp of the last modification (suffix required: 21180128).
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the reviewed app.
    /// </summary>
    public virtual App App { get; set; } = null!;

    /// <summary>
    /// Navigation property to the user who wrote the review (nullable).
    /// </summary>
    public virtual User? User { get; set; }
}
