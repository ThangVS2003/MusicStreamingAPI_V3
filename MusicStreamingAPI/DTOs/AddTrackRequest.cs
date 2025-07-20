namespace MusicStreamingAPI.DTOs
{
    public class AddTrackRequest
    {
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public int? AlbumId { get; set; }
        public int? CategoryId { get; set; }
        public int Duration { get; set; }
        public IFormFile File { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Lyrics { get; set; }
        public bool IsPublic { get; set; } = true;
    }
    public class UpdateTrackRequest
    {
        public string? Title { get; set; }
        public string? ArtistName { get; set; }
        public int? AlbumId { get; set; }
        public int? CategoryId { get; set; }
        public int? Duration { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Lyrics { get; set; }
        public bool? IsPublic { get; set; }
    }
}
