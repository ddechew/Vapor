namespace VaporWebAPI.Models;

/// <summary>
/// Represents a software application (game, DLC, soundtrack, etc.) in the Vapor store.
/// </summary>
public partial class App
{
    /// <summary>
    /// Unique identifier of the app.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Type of the app (e.g., game, DLC, demo).
    /// </summary>
    public int AppTypeId { get; set; }

    /// <summary>
    /// If the app is additional content, this links to its base game.
    /// </summary>
    public int? BaseAppId { get; set; }

    /// <summary>
    /// Name/title of the app.
    /// </summary>
    public string AppName { get; set; } = null!;

    /// <summary>
    /// Release date as a string (Steam format).
    /// </summary>
    public string ReleaseDate { get; set; } = null!;

    /// <summary>
    /// Price of the app in euros. Null if not available.
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Short description of the app.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Number of times the app has been purchased.
    /// </summary>
    public int? PurchaseCount { get; set; }

    /// <summary>
    /// Last time this app record was modified. (Suffix 21180128 as required)
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Developers associated with this app.
    /// </summary>
    public virtual ICollection<AppDeveloper> AppDevelopers { get; set; } = new List<AppDeveloper>();

    /// <summary>
    /// Genres associated with this app.
    /// </summary>
    public virtual ICollection<AppGenre> AppGenres { get; set; } = new List<AppGenre>();

    /// <summary>
    /// Images associated with this app (header, screenshots, etc.).
    /// </summary>
    public virtual ICollection<AppImage> AppImages { get; set; } = new List<AppImage>();

    /// <summary>
    /// Library entries where users own this app.
    /// </summary>
    public virtual ICollection<AppLibrary> AppLibraries { get; set; } = new List<AppLibrary>();

    /// <summary>
    /// Publishers associated with this app.
    /// </summary>
    public virtual ICollection<AppPublisher> AppPublishers { get; set; } = new List<AppPublisher>();

    /// <summary>
    /// Reviews posted for this app.
    /// </summary>
    public virtual ICollection<AppReview> AppReviews { get; set; } = new List<AppReview>();

    /// <summary>
    /// Type definition of the app (foreign key to AppType).
    /// </summary>
    public virtual AppType AppType { get; set; } = null!;

    /// <summary>
    /// Video trailers or clips associated with the app.
    /// </summary>
    public virtual ICollection<AppVideo> AppVideos { get; set; } = new List<AppVideo>();

    /// <summary>
    /// Cart entries where this app is added by users.
    /// </summary>
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    /// <summary>
    /// Community posts related to this app.
    /// </summary>
    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();

    /// <summary>
    /// Wishlist entries where this app is wished by users.
    /// </summary>
    public virtual ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();
}
