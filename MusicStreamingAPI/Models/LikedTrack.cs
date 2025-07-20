using System;
using System.Collections.Generic;

namespace MusicStreamingAPI.Models;

public partial class LikedTrack
{
    public int LikeId { get; set; }

    public int UserId { get; set; }

    public int SoundId { get; set; }

    public DateTime? LikedAt { get; set; }

    public virtual Sound Sound { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
