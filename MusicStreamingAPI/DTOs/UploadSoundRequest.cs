namespace MusicStreamingAPI.DTOs
{
    public class UploadSoundRequest
    {
        public string Title { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public int? AlbumId { get; set; }
        public int? CategoryId { get; set; }
        public int Duration { get; set; }
        public IFormFile File { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public string? Lyrics { get; set; }
        public int UploadedBy { get; set; }
        public bool? IsPublic { get; set; }
    }
}