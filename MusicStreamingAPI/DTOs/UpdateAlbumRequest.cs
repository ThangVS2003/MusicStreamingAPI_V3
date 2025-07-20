namespace MusicStreamingAPI.DTOs
{
    public class UpdateAlbumRequest
    {
        public string? AlbumTitle { get; set; }
        public string? ArtistName { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public string? Genre { get; set; }
    }
}