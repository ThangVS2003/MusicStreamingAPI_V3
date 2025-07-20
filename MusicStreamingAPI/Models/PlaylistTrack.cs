using System;
using System.Collections.Generic;

namespace MusicStreamingAPI.Models;

public partial class PlaylistTrack
{
    public int PlaylistTrackId { get; set; }

    public int PlaylistId { get; set; }

    public int SoundId { get; set; }

    public int TrackOrder { get; set; }

    public DateTime? AddedAt { get; set; }

    public virtual Playlist Playlist { get; set; } = null!;

    public virtual Sound Sound { get; set; } = null!;
}
