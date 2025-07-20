namespace MusicStreamingAPI.DTOs
{
    public class CreatePlaylistRequest
    {
        public string PlaylistName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int UserId { get; set; }
        public string? CoverImageUrl { get; set; }
        public bool IsPublic { get; set; } = true;
    }
}