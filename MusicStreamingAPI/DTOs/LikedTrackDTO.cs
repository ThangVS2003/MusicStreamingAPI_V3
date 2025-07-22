namespace MusicStreamingAPI.DTOs
{
    public class LikedTrackDTO
    {
        public int UserId { get; set; }
        public SoundDTO Sound { get; set; }
    }

    public class SoundDTO
    {
        public int SoundId { get; set; }
    }
}