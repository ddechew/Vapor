namespace VaporWebAPI.DTOs.AdminDTOs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data Transfer Object used by admins to view or modify user information.
/// </summary>
public class UserAdminDTO
{
    /// <summary>
    /// Unique identifier of the user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Username of the user (must be unique).
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User's password (used for creation or reset).
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Display name shown on posts and comments.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Email address of the user.
    /// </summary>
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Current wallet balance in euros.
    /// </summary>
    public decimal Wallet { get; set; }

    /// <summary>
    /// Total points earned by the user.
    /// </summary>
    public int Points { get; set; }

    /// <summary>
    /// Foreign key referencing the user's role (e.g., 1 = User, 2 = Admin).
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Name of the role assigned to the user.
    /// </summary>
    public string RoleName { get; set; } = string.Empty;
}
