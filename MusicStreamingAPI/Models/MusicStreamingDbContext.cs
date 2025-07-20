using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MusicStreamingAPI.Models;

public partial class MusicStreamingDbContext : DbContext
{
    public MusicStreamingDbContext()
    {
    }

    public MusicStreamingDbContext(DbContextOptions<MusicStreamingDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminLog> AdminLogs { get; set; }

    public virtual DbSet<Album> Albums { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<LikedTrack> LikedTracks { get; set; }

    public virtual DbSet<ListeningHistory> ListeningHistories { get; set; }

    public virtual DbSet<Playlist> Playlists { get; set; }

    public virtual DbSet<PlaylistTrack> PlaylistTracks { get; set; }

    public virtual DbSet<SearchHistory> SearchHistories { get; set; }

    public virtual DbSet<Sound> Sounds { get; set; }

    public virtual DbSet<SystemStat> SystemStats { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserFollow> UserFollows { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=localhost;database=MusicStreamingDB;uid=sa;pwd=sa;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__AdminLog__5E548648C13107CC");

            entity.Property(e => e.Action).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Details).HasMaxLength(1000);
            entity.Property(e => e.TargetType).HasMaxLength(50);

            entity.HasOne(d => d.AdminUser).WithMany(p => p.AdminLogs)
                .HasForeignKey(d => d.AdminUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AdminLogs__Admin__76969D2E");
        });

        modelBuilder.Entity<Album>(entity =>
        {
            entity.HasKey(e => e.AlbumId).HasName("PK__Albums__97B4BE3733084AB6");

            entity.Property(e => e.AlbumTitle).HasMaxLength(200);
            entity.Property(e => e.ArtistName).HasMaxLength(100);
            entity.Property(e => e.CoverImageUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Duration).HasDefaultValue(0);
            entity.Property(e => e.Genre).HasMaxLength(50);
            entity.Property(e => e.TotalTracks).HasDefaultValue(0);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PK__Categori__19093A0BDD319D07");

            entity.Property(e => e.CategoryName).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.ImageUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PK__Comments__C3B4DFCA34F3F68D");

            entity.Property(e => e.CommentText).HasMaxLength(1000);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.ParentComment).WithMany(p => p.InverseParentComment)
                .HasForeignKey(d => d.ParentCommentId)
                .HasConstraintName("FK__Comments__Parent__778AC167");

            entity.HasOne(d => d.Sound).WithMany(p => p.Comments)
                .HasForeignKey(d => d.SoundId)
                .HasConstraintName("FK__Comments__SoundI__787EE5A0");

            entity.HasOne(d => d.User).WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Comments__UserId__797309D9");
        });

        modelBuilder.Entity<LikedTrack>(entity =>
        {
            entity.HasKey(e => e.LikeId).HasName("PK__LikedTra__A2922C148E07A101");

            entity.HasIndex(e => new { e.UserId, e.SoundId }, "UQ__LikedTra__16F34EDBBFCCD88D").IsUnique();

            entity.Property(e => e.LikedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Sound).WithMany(p => p.LikedTracks)
                .HasForeignKey(d => d.SoundId)
                .HasConstraintName("FK__LikedTrac__Sound__7A672E12");

            entity.HasOne(d => d.User).WithMany(p => p.LikedTracks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__LikedTrac__UserI__7B5B524B");
        });

        modelBuilder.Entity<ListeningHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__Listenin__4D7B4ABD3F772359");

            entity.ToTable("ListeningHistory");

            entity.Property(e => e.CompletedPlay).HasDefaultValue(false);
            entity.Property(e => e.PlayedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Sound).WithMany(p => p.ListeningHistories)
                .HasForeignKey(d => d.SoundId)
                .HasConstraintName("FK__Listening__Sound__7C4F7684");

            entity.HasOne(d => d.User).WithMany(p => p.ListeningHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Listening__UserI__7D439ABD");
        });

        modelBuilder.Entity<Playlist>(entity =>
        {
            entity.HasKey(e => e.PlaylistId).HasName("PK__Playlist__B30167A00AC7D773");

            entity.Property(e => e.CoverImageUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.IsPublic).HasDefaultValue(true);
            entity.Property(e => e.PlaylistName).HasMaxLength(200);
            entity.Property(e => e.TotalDuration).HasDefaultValue(0);
            entity.Property(e => e.TotalTracks).HasDefaultValue(0);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.Playlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Playlists__UserI__7E37BEF6");
        });

        modelBuilder.Entity<PlaylistTrack>(entity =>
        {
            entity.HasKey(e => e.PlaylistTrackId).HasName("PK__Playlist__70B9B2EA625F565A");

            entity.HasIndex(e => new { e.PlaylistId, e.SoundId }, "UQ__Playlist__B27AE537E829FCB7").IsUnique();

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Playlist).WithMany(p => p.PlaylistTracks)
                .HasForeignKey(d => d.PlaylistId)
                .HasConstraintName("FK__PlaylistT__Playl__7F2BE32F");

            entity.HasOne(d => d.Sound).WithMany(p => p.PlaylistTracks)
                .HasForeignKey(d => d.SoundId)
                .HasConstraintName("FK__PlaylistT__Sound__00200768");
        });

        modelBuilder.Entity<SearchHistory>(entity =>
        {
            entity.HasKey(e => e.SearchId).HasName("PK__SearchHi__21C535F41A6216BE");

            entity.ToTable("SearchHistory");

            entity.Property(e => e.ResultCount).HasDefaultValue(0);
            entity.Property(e => e.SearchQuery).HasMaxLength(200);
            entity.Property(e => e.SearchType).HasMaxLength(50);
            entity.Property(e => e.SearchedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.User).WithMany(p => p.SearchHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__SearchHis__UserI__01142BA1");
        });

        modelBuilder.Entity<Sound>(entity =>
        {
            entity.HasKey(e => e.SoundId).HasName("PK__Sounds__17B8296155DFC998");

            entity.Property(e => e.ArtistName).HasMaxLength(100);
            entity.Property(e => e.CoverImageUrl).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsPublic).HasDefaultValue(true);
            entity.Property(e => e.LikeCount).HasDefaultValue(0);
            entity.Property(e => e.PlayCount).HasDefaultValue(0);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Album).WithMany(p => p.Sounds)
                .HasForeignKey(d => d.AlbumId)
                .HasConstraintName("FK__Sounds__AlbumId__02084FDA");

            entity.HasOne(d => d.Category).WithMany(p => p.Sounds)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Sounds__Category__02FC7413");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Sounds)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("FK__Sounds__Uploaded__03F0984C");
        });

        modelBuilder.Entity<SystemStat>(entity =>
        {
            entity.HasKey(e => e.StatId).HasName("PK__SystemSt__3A162D3EA35AD38F");

            entity.HasIndex(e => e.StatDate, "UQ__SystemSt__255A932DDDA704A8").IsUnique();

            entity.Property(e => e.ActiveUsers).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TotalAlbums).HasDefaultValue(0);
            entity.Property(e => e.TotalPlaylists).HasDefaultValue(0);
            entity.Property(e => e.TotalPlays).HasDefaultValue(0);
            entity.Property(e => e.TotalSounds).HasDefaultValue(0);
            entity.Property(e => e.TotalUsers).HasDefaultValue(0);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C9F96A282");

            entity.HasIndex(e => e.Username, "UQ__Users__536C85E4A40C82DE").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Users__A9D105345BE07DDB").IsUnique();

            entity.Property(e => e.AvatarUrl).HasMaxLength(500);
            entity.Property(e => e.Bio).HasMaxLength(1000);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(10);
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsAdmin).HasDefaultValue(false);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        modelBuilder.Entity<UserFollow>(entity =>
        {
            entity.HasKey(e => e.FollowId).HasName("PK__UserFoll__2CE810AE4019D324");

            entity.HasIndex(e => new { e.FollowerId, e.FollowingId }, "UQ__UserFoll__79CB03343B9BD497").IsUnique();

            entity.Property(e => e.FollowedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Follower).WithMany(p => p.UserFollowFollowers)
                .HasForeignKey(d => d.FollowerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserFollo__Follo__04E4BC85");

            entity.HasOne(d => d.Following).WithMany(p => p.UserFollowFollowings)
                .HasForeignKey(d => d.FollowingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserFollo__Follo__05D8E0BE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
