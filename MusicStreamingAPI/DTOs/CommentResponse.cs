namespace MusicStreamingAPI.DTOs
{
    public class CommentResponse
    {
        public int CommentId { get; set; }
        public int SoundId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string CommentText { get; set; } = string.Empty;
        public int? ParentCommentId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CommentResponse> ChildComments { get; set; } = new List<CommentResponse>();
    }
}