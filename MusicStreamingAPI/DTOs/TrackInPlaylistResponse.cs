namespace MusicStreamingAPI.DTOs
{
    public class TrackInPlaylistResponse
    {
        public int SoundId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
    
        public string? CoverImageUrl { get; set; } // Thêm trường URL ảnh bìa
    }
}
