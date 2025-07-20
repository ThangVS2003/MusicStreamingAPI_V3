namespace MusicStreamingAPI.DTOs
{
    public class CreateAlbumRequest
    {
        public string AlbumTitle { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public string? Genre { get; set; }
    }
}