using System;
using System.Collections.Generic;

namespace MusicStreamingAPI.Models;

public partial class UserFollow
{
    public int FollowId { get; set; }

    public int FollowerId { get; set; }

    public int FollowingId { get; set; }

    public DateTime? FollowedAt { get; set; }

    public virtual User Follower { get; set; } = null!;

    public virtual User Following { get; set; } = null!;
}
