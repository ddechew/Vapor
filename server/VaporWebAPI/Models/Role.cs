namespace VaporWebAPI.Models;

/// <summary>
/// Represents a user role in the system (e.g., Admin, User).
/// </summary>
public partial class Role
{
    /// <summary>
    /// Primary key identifier for the role.
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// The name of the role (e.g., "Admin", "User").
    /// </summary>
    public string RoleName { get; set; } = null!;

    /// <summary>
    /// Timestamp of the last modification for this role record.
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Collection of users assigned to this role.
    /// </summary>
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
