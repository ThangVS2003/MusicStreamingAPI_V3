namespace MusicStreamingAPI.DTOs
{
    public class LikedTrackDTO
    {

        public int UserId { get; set; }

        public int SoundId { get; set; }

        public DateTime? LikedAt { get; set; }

        // Optional: Tùy bạn có muốn include thông tin user/sound hay không
    }
}
