namespace VaporWebAPI.Models;

/// <summary>
/// Represents a log entry for data manipulation operations (INSERT, UPDATE) on database tables.
/// Used for auditing changes within the system.
/// </summary>
public partial class Log21180128
{
    /// <summary>
    /// Primary key of the log entry.
    /// </summary>
    public int LogId { get; set; }

    /// <summary>
    /// The name of the table where the operation occurred.
    /// </summary>
    public string TableName { get; set; } = null!;

    /// <summary>
    /// Type of operation performed, e.g., "INSERT" or "UPDATE".
    /// </summary>
    public string OperationType { get; set; } = null!;

    /// <summary>
    /// Timestamp when the operation was executed.
    /// </summary>
    public DateTime? OperationTimestamp { get; set; }

    /// <summary>
    /// Optional identifier of the user or process that modified the data.
    /// </summary>
    public string? ModifiedBy { get; set; }
}
