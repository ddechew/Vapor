namespace VaporWebAPI.Models;

/// <summary>
/// Represents a game genre/category, such as Action, RPG, Strategy, etc.
/// </summary>
public partial class Genre
{
    /// <summary>
    /// Primary key for the Genre entity.
    /// </summary>
    public int GenreId { get; set; }

    /// <summary>
    /// Name of the genre.
    /// </summary>
    public string GenreName { get; set; } = null!;

    /// <summary>
    /// Optional external genre ID (e.g., from Steam or another external source).
    /// </summary>
    public int? ExternalGenreId { get; set; }

    /// <summary>
    /// Timestamp for last modification, following the naming convention with suffix 21180128.
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property for the related apps associated with this genre.
    /// </summary>
    public virtual ICollection<AppGenre> AppGenres { get; set; } = new List<AppGenre>();
}
