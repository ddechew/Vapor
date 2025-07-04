namespace VaporWebAPI.DTOs;

/// <summary>
/// Data Transfer Object for creating or updating a review for an app.
/// </summary>
public class CreateReviewRequest
{
    /// <summary>
    /// The ID of the app being reviewed.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Indicates whether the user recommends the app.
    /// </summary>
    public bool IsRecommended { get; set; }

    /// <summary>
    /// The text content of the review.
    /// </summary>
    public string ReviewText { get; set; } = null!;

    /// <summary>
    /// Indicates whether the review has been edited after creation.
    /// This is usually managed by the server, not set by the client.
    /// </summary>
    public bool IsEdited { get; set; } = false;
}
