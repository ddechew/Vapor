namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object representing a review for an application in the admin panel.
/// </summary>
public class AppReviewAdminDTO
{
    /// <summary>
    /// Unique identifier of the review.
    /// </summary>
    public int ReviewId { get; set; }

    /// <summary>
    /// Identifier of the associated application.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Name of the associated application.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// Identifier of the user who wrote the review. Nullable if user is deleted.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Display name of the user who wrote the review.
    /// </summary>
    public string UserDisplayName { get; set; }

    /// <summary>
    /// Indicates whether the user recommends the application.
    /// </summary>
    public bool IsRecommended { get; set; }

    /// <summary>
    /// Text content of the review.
    /// </summary>
    public string ReviewText { get; set; }

    /// <summary>
    /// Date and time when the review was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indicates whether the review was edited after creation.
    /// </summary>
    public bool IsEdited { get; set; }
}
