namespace VaporWebAPI.Models;

/// <summary>
/// Join entity representing the many-to-many relationship between Apps and Genres.
/// </summary>
public partial class AppGenre
{
    /// <summary>
    /// ID of the associated app.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// ID of the associated genre.
    /// </summary>
    public int GenreId { get; set; }

    /// <summary>
    /// Timestamp of the last modification (required suffix 21180128).
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property for the associated App.
    /// </summary>
    public virtual App App { get; set; } = null!;

    /// <summary>
    /// Navigation property for the associated Genre.
    /// </summary>
    public virtual Genre Genre { get; set; } = null!;
}
