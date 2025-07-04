namespace VaporWebAPI.Data;

using Microsoft.EntityFrameworkCore;

using VaporWebAPI.Models;

/// <summary>
/// Represents the Entity Framework Core database context for the Vapor application.
/// This context manages the database tables (entities) and configures relationships, constraints, and triggers
/// according to the schema "21180128".
/// </summary>
public partial class VaporDbContext : DbContext
{
    /// <summary>
    /// Parameterless constructor.
    /// </summary>
    public VaporDbContext()
    {
    }

    /// <summary>
    /// Constructor accepting options for configuration.
    /// </summary>
    /// <param name="options">The DbContext options.</param>
    public VaporDbContext(DbContextOptions<VaporDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<App> Apps { get; set; }

    public virtual DbSet<AppDeveloper> AppDevelopers { get; set; }

    public virtual DbSet<AppGenre> AppGenres { get; set; }

    public virtual DbSet<AppImage> AppImages { get; set; }

    public virtual DbSet<AppLibrary> AppLibraries { get; set; }

    public virtual DbSet<AppPublisher> AppPublishers { get; set; }

    public virtual DbSet<AppReview> AppReviews { get; set; }

    public virtual DbSet<AppType> AppTypes { get; set; }

    public virtual DbSet<AppVideo> AppVideos { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Developer> Developers { get; set; }

    public virtual DbSet<EmailChangeToken> EmailChangeTokens { get; set; }

    public virtual DbSet<EmailVerification> EmailVerifications { get; set; }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<Log21180128> Log21180128s { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    public virtual DbSet<PaymentToken> PaymentTokens { get; set; }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<PostComment> PostComments { get; set; }

    public virtual DbSet<PostLike> PostLikes { get; set; }

    public virtual DbSet<Publisher> Publishers { get; set; }

    public virtual DbSet<PurchaseHistory> PurchaseHistories { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Wishlist> Wishlists { get; set; }

    /// <summary>
    /// Configures the entity mappings, relationships, keys, constraints, and triggers.
    /// All tables are mapped within the "21180128" schema.
    /// Default values and datetime columns are configured here.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure the entities.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<App>(entity =>
        {
            entity.HasKey(e => e.AppId).HasName("PK__Apps__8E2CF7F93D305941");

            entity.ToTable("Apps", "21180128", tb =>
                {
                    tb.HasTrigger("trg_Apps_Insert_Log");
                    tb.HasTrigger("trg_Apps_Update_LastModification_Log");
                });

            entity.Property(e => e.AppId).ValueGeneratedNever();
            entity.Property(e => e.AppName).HasMaxLength(255);
            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PurchaseCount).HasDefaultValue(0);

            entity.HasOne(d => d.AppType).WithMany(p => p.Apps)
                .HasForeignKey(d => d.AppTypeId)
                .HasConstraintName("FK_Apps_AppTypes");
        });

        modelBuilder.Entity<AppDeveloper>(entity =>
        {
            entity.HasKey(e => new { e.AppId, e.DeveloperId }).HasName("PK__AppDevel__83CC73369C7B9F2F");

            entity.ToTable("AppDevelopers", "21180128", tb =>
                {
                    tb.HasTrigger("trg_AppDevelopers_Insert_Log");
                    tb.HasTrigger("trg_AppDevelopers_Update_LastModification_Log");
                });

            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.App).WithMany(p => p.AppDevelopers)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("FK_AppDevelopers_Apps");

            entity.HasOne(d => d.Developer).WithMany(p => p.AppDevelopers)
                .HasForeignKey(d => d.DeveloperId)
                .HasConstraintName("FK_AppDevelopers_Developers");
        });

        modelBuilder.Entity<AppGenre>(entity =>
        {
            entity.HasKey(e => new { e.AppId, e.GenreId });

            entity.ToTable("AppGenres", "21180128", tb =>
                {
                    tb.HasTrigger("trg_AppGenres_Insert_Log");
                    tb.HasTrigger("trg_AppGenres_Update_LastModification_Log");
                });

            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.App).WithMany(p => p.AppGenres)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("FK_AppGenres_Apps");

            entity.HasOne(d => d.Genre).WithMany(p => p.AppGenres)
                .HasForeignKey(d => d.GenreId)
                .HasConstraintName("FK_AppGenres_Genres");
        });

        modelBuilder.Entity<AppImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__AppImage__7516F70CEE4E29BC");

            entity.ToTable("AppImages", "21180128", tb =>
                {
                    tb.HasTrigger("trg_AppImages_Insert_Log");
                    tb.HasTrigger("trg_AppImages_Update_LastModification_Log");
                });

            entity.Property(e => e.ImageType).HasMaxLength(50);
            entity.Property(e => e.ImageUrl).HasMaxLength(1000);
            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(1000);

            entity.HasOne(d => d.App).WithMany(p => p.AppImages)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("FK_AppImages_Apps");
        });

        modelBuilder.Entity<AppLibrary>(entity =>
        {
            entity.HasKey(e => e.LibraryId).HasName("PK__AppLibra__A136475F26317098");

            entity.ToTable("AppLibrary", "21180128", tb =>
                {
                    tb.HasTrigger("trg_AppLibrary_Insert_Log");
                    tb.HasTrigger("trg_AppLibrary_Update_LastModification_Log");
                });

            entity.HasIndex(e => new { e.UserId, e.AppId }, "UQ_AppLibrary_UserApp").IsUnique();

            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PurchaseDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.App).WithMany(p => p.AppLibraries)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("FK_AppLibrary_Apps");

            entity.HasOne(d => d.User).WithMany(p => p.AppLibraries)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AppLibrary_Users");
        });

        modelBuilder.Entity<AppPublisher>(entity =>
        {
            entity.HasKey(e => new { e.AppId, e.PublisherId }).HasName("PK__AppPubli__2AEAA00395FE316E");

            entity.ToTable("AppPublishers", "21180128", tb =>
                {
                    tb.HasTrigger("trg_AppPublishers_Insert_Log");
                    tb.HasTrigger("trg_AppPublishers_Update_LastModification_Log");
                });

            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.App).WithMany(p => p.AppPublishers)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("FK_AppPublishers_Apps");

            entity.HasOne(d => d.Publisher).WithMany(p => p.AppPublishers)
                .HasForeignKey(d => d.PublisherId)
                .HasConstraintName("FK_AppPublishers_Publishers");
        });

        modelBuilder.Entity<AppReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__AppRevie__74BC79CEEB18F4AE");

            entity.ToTable("AppReviews", "21180128", tb =>
                {
                    tb.HasTrigger("trg_AppReviews_Insert_Log");
                    tb.HasTrigger("trg_AppReviews_Update_LastModification_Log");
                });

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserDisplayName).HasMaxLength(100);

            entity.HasOne(d => d.App).WithMany(p => p.AppReviews)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("FK_AppReviews_Apps");

            entity.HasOne(d => d.User).WithMany(p => p.AppReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_AppReviews_Users");
        });

        modelBuilder.Entity<AppType>(entity =>
        {
            entity.HasKey(e => e.AppTypeId).HasName("PK__AppTypes__14129742474C69B6");

            entity.ToTable("AppTypes", "21180128", tb =>
                {
                    tb.HasTrigger("trg_AppTypes_Insert_Log");
                    tb.HasTrigger("trg_AppTypes_Update_LastModification_Log");
                });

            entity.HasIndex(e => e.TypeName, "UQ__AppTypes__D4E7DFA8441755E2").IsUnique();

            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TypeName).HasMaxLength(50);
        });

        modelBuilder.Entity<AppVideo>(entity =>
        {
            entity.HasKey(e => e.VideoId).HasName("PK__AppVideo__BAE5126A87A25C44");

            entity.ToTable("AppVideos", "21180128", tb =>
                {
                    tb.HasTrigger("trg_AppVideos_Insert_Log");
                    tb.HasTrigger("trg_AppVideos_Update_LastModification_Log");
                });

            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ThumbnailUrl).HasMaxLength(1000);
            entity.Property(e => e.VideoUrl).HasMaxLength(1000);

            entity.HasOne(d => d.App).WithMany(p => p.AppVideos)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("FK_AppVideos_Apps");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.CartItemId).HasName("PK__CartItem__488B0B0A1F4548D6");

            entity.ToTable("CartItems", "21180128", tb =>
                {
                    tb.HasTrigger("trg_CartItems_Insert_Log");
                    tb.HasTrigger("trg_CartItems_Update_LastModification_Log");
                });

            entity.Property(e => e.DateAdded)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.App).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("FK_CartItems_AppId");

            entity.HasOne(d => d.User).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_CartItems_Users");
        });

        modelBuilder.Entity<Developer>(entity =>
        {
            entity.HasKey(e => e.DeveloperId).HasName("PK__Develope__DE084CF1C46376C7");

            entity.ToTable("Developers", "21180128", tb =>
                {
                    tb.HasTrigger("trg_Developers_Insert_Log");
                    tb.HasTrigger("trg_Developers_Update_LastModification_Log");
                });

            entity.HasIndex(e => e.DeveloperName, "UQ__Develope__08E3F54D9A5BF962").IsUnique();

            entity.Property(e => e.DeveloperName).HasMaxLength(255);
            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<EmailChangeToken>(entity =>
        {
            entity.HasKey(e => e.EmailChangeTokenId).HasName("PK__EmailCha__8A3DB49C773930E4");

            entity.ToTable("EmailChangeTokens", "21180128", tb =>
                {
                    tb.HasTrigger("trg_EmailChangeTokens_Insert_Log");
                    tb.HasTrigger("trg_EmailChangeTokens_Update_LastModification_Log");
                });

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.LastModification21180128).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.NewEmail).HasMaxLength(255);
            entity.Property(e => e.Token).HasMaxLength(64);

            entity.HasOne(d => d.User).WithMany(p => p.EmailChangeTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_EmailChangeTokens_Users");
        });

        modelBuilder.Entity<EmailVerification>(entity =>
        {
            entity.HasKey(e => e.VerificationId).HasName("PK__EmailVer__306D4907430A563D");

            entity.ToTable("EmailVerifications", "21180128", tb =>
                {
                    tb.HasTrigger("trg_EmailVerifications_Insert_Log");
                    tb.HasTrigger("trg_EmailVerifications_Update_LastModification_Log");
                });

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(255);

            entity.HasOne(d => d.User).WithMany(p => p.EmailVerifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_EmailVeri__UserI__503BEA1C");
        });

        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK__Genres__0385057EDD29F744");

            entity.ToTable("Genres", "21180128", tb =>
                {
                    tb.HasTrigger("trg_Genres_Insert_Log");
                    tb.HasTrigger("trg_Genres_Update_LastModification_Log");
                });

            entity.HasIndex(e => e.GenreName, "UQ__Genres__BBE1C33946121290").IsUnique();

            entity.Property(e => e.GenreName).HasMaxLength(100);
            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        modelBuilder.Entity<Log21180128>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__log_2118__5E548648AD92015B");

            entity.ToTable("log_21180128", "21180128");

            entity.Property(e => e.ModifiedBy)
                .HasMaxLength(100)
                .HasDefaultValueSql("(original_login())");
            entity.Property(e => e.OperationTimestamp)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.OperationType).HasMaxLength(50);
            entity.Property(e => e.TableName).HasMaxLength(255);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E1260FEE1D7");

            entity.ToTable("Notifications", "21180128", tb =>
                {
                    tb.HasTrigger("trg_Notifications_Insert_Log");
                    tb.HasTrigger("trg_Notifications_Update_LastModification_Log");
                });

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Message).HasMaxLength(255);

            entity.HasOne(d => d.Post).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.PostId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Notifications_Posts");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Notifications_User_Receiver");
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Password__3214EC079718FD7F");

            entity.ToTable("PasswordResetTokens", "21180128", tb =>
                {
                    tb.HasTrigger("trg_PasswordResetTokens_Insert_Log");
                    tb.HasTrigger("trg_PasswordResetTokens_Update_LastModification_Log");
                });

            entity.HasIndex(e => e.Token, "UQ__Password__1EB4F8176CE1B2B7").IsUnique();

            entity.Property(e => e.ExpirationTime).HasColumnType("datetime");
            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Token).HasMaxLength(128);

            entity.HasOne(d => d.User).WithMany(p => p.PasswordResetTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__PasswordR__UserI__339FAB6E");
        });

        modelBuilder.Entity<PaymentToken>(entity =>
        {
            entity.HasKey(e => e.PaymentTokenId).HasName("PK__PaymentT__42A5D48FED241C64");

            entity.ToTable("PaymentTokens", "21180128", tb =>
                {
                    tb.HasTrigger("trg_PaymentTokens_Insert_Log");
                    tb.HasTrigger("trg_PaymentTokens_Update_LastModification_Log");
                });

            entity.HasIndex(e => e.Token, "UQ__PaymentT__1EB4F817B01BADE7").IsUnique();

            entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.LastModification21180128).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.SessionId).HasMaxLength(255);
            entity.Property(e => e.Token).HasMaxLength(100);

            entity.HasOne(d => d.User).WithMany(p => p.PaymentTokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_PaymentTokens_Users");
        });

        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => e.PostId).HasName("PK__Posts__AA126018F20C4A28");

            entity.ToTable("Posts", "21180128", tb =>
                {
                    tb.HasTrigger("trg_Posts_Insert_Log");
                    tb.HasTrigger("trg_Posts_Update_LastModification_Log");
                });

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserDisplayName)
                .HasMaxLength(100)
                .HasDefaultValue("Deleted User");

            entity.HasOne(d => d.App).WithMany(p => p.Posts)
                .HasForeignKey(d => d.AppId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Posts_Apps");

            entity.HasOne(d => d.User).WithMany(p => p.Posts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_Posts_Users");
        });

        modelBuilder.Entity<PostComment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__PostComm__C3B4DFCA8C0E9AD8");

            entity.ToTable("PostComments", "21180128", tb =>
                {
                    tb.HasTrigger("trg_PostComments_Insert_Log");
                    tb.HasTrigger("trg_PostComments_Update_LastModification_Log");
                });

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.UserDisplayName)
                .HasMaxLength(100)
                .HasDefaultValue("Deleted User");

            entity.HasOne(d => d.Post).WithMany(p => p.PostComments)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_PostComments_Posts");

            entity.HasOne(d => d.User).WithMany(p => p.PostComments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PostComments_Users");
        });

        modelBuilder.Entity<PostLike>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__PostLike__A2922C14A43681B1");

            entity.ToTable("PostLikes", "21180128", tb =>
                {
                    tb.HasTrigger("trg_PostLikes_Insert_Log");
                    tb.HasTrigger("trg_PostLikes_Update_LastModification_Log");
                });

            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Post).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.PostId)
                .HasConstraintName("FK_PostLikes_Posts");

            entity.HasOne(d => d.User).WithMany(p => p.PostLikes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PostLikes_Users");
        });

        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.PublisherId).HasName("PK__Publishe__4C657FABDD8591FF");

            entity.ToTable("Publishers", "21180128", tb =>
                {
                    tb.HasTrigger("trg_Publishers_Insert_Log");
                    tb.HasTrigger("trg_Publishers_Update_LastModification_Log");
                });

            entity.HasIndex(e => e.PublisherName, "UQ__Publishe__5F0E224951EB4CC5").IsUnique();

            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PublisherName).HasMaxLength(255);
        });

        modelBuilder.Entity<PurchaseHistory>(entity =>
        {
            entity.HasKey(e => e.PurchaseId).HasName("PK__Purchase__6B0A6BBEE872DA6F");

            entity.ToTable("PurchaseHistory", "21180128", tb =>
                {
                    tb.HasTrigger("trg_PurchaseHistory_Insert_Log");
                    tb.HasTrigger("trg_PurchaseHistory_Update_LastModification_Log");
                });

            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.PriceAtPurchase).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.PurchaseDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.WalletBalanceAfter).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.WalletChange).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.User).WithMany(p => p.PurchaseHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Purchase_User");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE1AC8515F25");

            entity.ToTable("Roles", "21180128", tb =>
                {
                    tb.HasTrigger("trg_Roles_Insert_Log");
                    tb.HasTrigger("trg_Roles_Update_LastModification_Log");
                });

            entity.HasIndex(e => e.RoleName, "UQ__Roles__8A2B6160287AA6C7").IsUnique();

            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.RoleName).HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users", "21180128", tb =>
                {
                    tb.HasTrigger("trg_Users_Insert_Log");
                    tb.HasTrigger("trg_Users_Update_LastModification_Log");
                });

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E466F94639").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D10534DC9DD0F8").IsUnique();

            entity.Property(e => e.DisplayName).HasMaxLength(255);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.GoogleId).HasMaxLength(128);
            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.PasswordHash).HasMaxLength(500);
            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(1000)
                .HasDefaultValue("D:\\repos\\Vapor\\client\\src\\assets\\defaultVaporProfilePic.jpg");
            entity.Property(e => e.RefreshToken).HasMaxLength(255);
            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.Wallet).HasColumnType("decimal(10, 2)");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_Users_Roles");
        });

        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.WishlistId).HasName("PK__Wishlist__233189EBEDB54E67");

            entity.ToTable("Wishlist", "21180128", tb =>
                {
                    tb.HasTrigger("trg_Wishlist_Insert_Log");
                    tb.HasTrigger("trg_Wishlist_Update_LastModification_Log");
                });

            entity.HasIndex(e => new { e.UserId, e.AppId }, "UQ_Wishlist_User_App").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.LastModification21180128)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.App).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.AppId)
                .HasConstraintName("FK_Wishlist_App");

            entity.HasOne(d => d.User).WithMany(p => p.Wishlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Wishlist_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    /// <summary>
    /// Partial method to allow further customization of model creation.
    /// </summary>
    /// <param name="modelBuilder"></param>
    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
