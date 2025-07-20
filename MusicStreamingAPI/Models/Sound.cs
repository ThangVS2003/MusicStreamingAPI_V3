using System;
using System.Collections.Generic;

namespace MusicStreamingAPI.Models;

public partial class Sound
{
    public int SoundId { get; set; }

    public string Title { get; set; } = null!;

    public string ArtistName { get; set; } = null!;

    public int? AlbumId { get; set; }

    public int? CategoryId { get; set; }

    public int Duration { get; set; }

    public string FileUrl { get; set; } = null!;

    public string? CoverImageUrl { get; set; }

    public string? Lyrics { get; set; }

    public int? PlayCount { get; set; }

    public int? LikeCount { get; set; }

    public bool? IsPublic { get; set; }

    public bool? IsActive { get; set; }

    public int? UploadedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Album? Album { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<LikedTrack> LikedTracks { get; set; } = new List<LikedTrack>();

    public virtual ICollection<ListeningHistory> ListeningHistories { get; set; } = new List<ListeningHistory>();

    public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; } = new List<PlaylistTrack>();

    public virtual User? UploadedByNavigation { get; set; }
}
