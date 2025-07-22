namespace MusicStreamingAPI.DTOs
{
    public class SoundResponseDto
    {
        public int SoundId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public int? AlbumId { get; set; }
        public int? CategoryId { get; set; }
        public int Duration { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public string? Lyrics { get; set; }
        public int? PlayCount { get; set; }
        public int? LikeCount { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsActive { get; set; }
        public int? UploadedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
