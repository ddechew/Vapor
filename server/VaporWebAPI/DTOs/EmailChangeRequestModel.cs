namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents the data needed to render an email change confirmation template.
/// </summary>
public class EmailChangeRequestModel
{
    /// <summary>
    /// The username of the user requesting the email change.
    /// </summary>
    public string Username { get; set; } = null!;

    /// <summary>
    /// The confirmation link sent to the user's current email address.
    /// </summary>
    public string Link { get; set; } = null!;

    /// <summary>
    /// The new email address the user wants to change to.
    /// </summary>
    public string NewEmail { get; set; } = null!;
}
