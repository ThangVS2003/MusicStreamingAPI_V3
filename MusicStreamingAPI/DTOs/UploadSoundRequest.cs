namespace MusicStreamingAPI.DTOs
{
    public class UploadSoundRequest
    {
        public string Title { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public int? UserId { get; set; }
    }

}