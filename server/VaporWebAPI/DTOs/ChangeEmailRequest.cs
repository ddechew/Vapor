namespace VaporWebAPI.DTOs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data Transfer Object used to initiate a user's email change request.
/// </summary>
public class ChangeEmailRequest
{
    /// <summary>
    /// The new email address that the user wants to associate with their account.
    /// </summary>
    [Required]
    [EmailAddress]
    public string NewEmail { get; set; }
}
