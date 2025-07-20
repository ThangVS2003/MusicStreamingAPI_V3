namespace MusicStreamingAPI.DTOs
{
    public class CreateCommentRequest
    {
        public int SoundId { get; set; }
        public int UserId { get; set; }
        public string CommentText { get; set; } = string.Empty;
        public int? ParentCommentId { get; set; }
    }
}