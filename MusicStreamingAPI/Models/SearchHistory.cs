using System;
using System.Collections.Generic;

namespace MusicStreamingAPI.Models;

public partial class SearchHistory
{
    public int SearchId { get; set; }

    public int? UserId { get; set; }

    public string SearchQuery { get; set; } = null!;

    public string? SearchType { get; set; }

    public int? ResultCount { get; set; }

    public DateTime? SearchedAt { get; set; }

    public virtual User? User { get; set; }
}
