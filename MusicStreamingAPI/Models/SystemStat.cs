using System;
using System.Collections.Generic;

namespace MusicStreamingAPI.Models;

public partial class SystemStat
{
    public int StatId { get; set; }

    public DateOnly StatDate { get; set; }

    public int? TotalUsers { get; set; }

    public int? ActiveUsers { get; set; }

    public int? TotalSounds { get; set; }

    public int? TotalPlaylists { get; set; }

    public int? TotalAlbums { get; set; }

    public int? TotalPlays { get; set; }

    public DateTime? CreatedAt { get; set; }
}
