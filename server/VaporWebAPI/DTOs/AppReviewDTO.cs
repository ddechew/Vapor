namespace VaporWebAPI.DTOs;

/// <summary>
/// Data Transfer Object representing a user review for an application.
/// </summary>
public class AppReviewDTO
{
    /// <summary>
    /// Unique identifier for the review.
    /// </summary>
    public int ReviewId { get; set; }


    /// <summary>
    /// ID of the application that the review is associated with.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Display name of the user who submitted the review.
    /// </summary>
    public string UserDisplayName { get; set; } = null!;

    /// <summary>
    /// Unique username of the reviewer. Can be null for deleted users.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// URL to the user's profile picture. Defaults to a placeholder if none is set.
    /// </summary>
    public string ProfilePictureUrl { get; set; }

    /// <summary>
    /// Indicates whether the user recommends the app (true = recommended).
    /// </summary>
    public bool IsRecommended { get; set; }

    /// <summary>
    /// The textual content of the review.
    /// </summary>
    public string ReviewText { get; set; } = null!;

    /// <summary>
    /// Date and time when the review was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indicates whether the review has been edited after creation.
    /// </summary>
    public bool IsEdited { get; set; }

}
