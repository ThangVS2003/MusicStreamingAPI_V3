namespace MusicStreamingAPI.DTOs
{
    public class PlaylistResponse
    {
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int UserId { get; set; }
        public string? CoverImageUrl { get; set; }
        public bool? IsPublic { get; set; }
        public int? TotalTracks { get; set; }
        public int? TotalDuration { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}