namespace VaporWebAPI.Models;

/// <summary>
/// Represents a publisher entity responsible for publishing apps.
/// </summary>
public partial class Publisher
{
    /// <summary>
    /// Primary key identifier for the publisher.
    /// </summary>
    public int PublisherId { get; set; }

    /// <summary>
    /// Name of the publisher.
    /// </summary>
    public string PublisherName { get; set; } = null!;

    /// <summary>
    /// Timestamp of the last modification for this record.
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the collection of app-publisher relationships.
    /// </summary>
    public virtual ICollection<AppPublisher> AppPublishers { get; set; } = new List<AppPublisher>();
}
