using System;
using System.Collections.Generic;

namespace MusicStreamingAPI.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int SoundId { get; set; }

    public int UserId { get; set; }

    public string CommentText { get; set; } = null!;

    public int? ParentCommentId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Comment> InverseParentComment { get; set; } = new List<Comment>();

    public virtual Comment? ParentComment { get; set; }

    public virtual Sound Sound { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
