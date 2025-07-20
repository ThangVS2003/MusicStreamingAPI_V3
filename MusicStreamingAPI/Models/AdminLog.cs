using System;
using System.Collections.Generic;

namespace MusicStreamingAPI.Models;

public partial class AdminLog
{
    public int LogId { get; set; }

    public int AdminUserId { get; set; }

    public string Action { get; set; } = null!;

    public string? TargetType { get; set; }

    public int? TargetId { get; set; }

    public string? Details { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User AdminUser { get; set; } = null!;
}
