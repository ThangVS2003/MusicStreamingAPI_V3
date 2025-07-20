using System;
using System.Collections.Generic;

namespace MusicStreamingAPI.Models;

public partial class ListeningHistory
{
    public int HistoryId { get; set; }

    public int UserId { get; set; }

    public int SoundId { get; set; }

    public DateTime? PlayedAt { get; set; }

    public int? PlayDuration { get; set; }

    public bool? CompletedPlay { get; set; }

    public virtual Sound Sound { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
