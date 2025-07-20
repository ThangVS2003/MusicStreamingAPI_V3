using System;
using System.Collections.Generic;

namespace MusicStreamingAPI.Models;

public partial class Playlist
{
    public int PlaylistId { get; set; }

    public string PlaylistName { get; set; } = null!;

    public string? Description { get; set; }

    public int UserId { get; set; }

    public string? CoverImageUrl { get; set; }

    public bool? IsPublic { get; set; }

    public int? TotalTracks { get; set; }

    public int? TotalDuration { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<PlaylistTrack> PlaylistTracks { get; set; } = new List<PlaylistTrack>();

    public virtual User User { get; set; } = null!;
}
