using System;
using System.Collections.Generic;

namespace MusicStreamingAPI.Models;

public partial class Album
{
    public int AlbumId { get; set; }

    public string AlbumTitle { get; set; } = null!;

    public string ArtistName { get; set; } = null!;

    public string? CoverImageUrl { get; set; }

    public DateOnly? ReleaseDate { get; set; }

    public string? Genre { get; set; }

    public int? TotalTracks { get; set; }

    public int? Duration { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Sound> Sounds { get; set; } = new List<Sound>();
}
