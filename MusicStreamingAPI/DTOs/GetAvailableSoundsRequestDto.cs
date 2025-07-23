namespace MusicStreamingAPI.DTOs
{
    public class GetAvailableSoundsRequestDto
    {
        public int UserId { get; set; }
        public int PlaylistId { get; set; }
    }
}
