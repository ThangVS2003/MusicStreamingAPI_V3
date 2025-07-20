using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.DTOs
{

    // DTOs
    public class LikeRequest
    {
        public int SoundId { get; set; }
    }
    public class LikedTrackDto
    {
        public int LikeId { get; set; }
        public int SoundId { get; set; }
        public DateTime? LikedAt { get; set; }
        public SoundDto Sound { get; set; }
        public LikedTrackDto(LikedTrack l)
        {
            LikeId = l.LikeId;
            SoundId = l.SoundId;
            LikedAt = l.LikedAt;
            Sound = new SoundDto(l.Sound);
        }
    }
   
}
