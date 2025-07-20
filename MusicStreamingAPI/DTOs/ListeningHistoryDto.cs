using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.DTOs
{
    public class ListeningHistoryDto
    {
        public int HistoryId { get; set; }
        public int SoundId { get; set; }
        public DateTime? PlayedAt { get; set; }
        public int? PlayDuration { get; set; }
        public bool? CompletedPlay { get; set; }
        public SoundDto Sound { get; set; }
        public ListeningHistoryDto(ListeningHistory h)
        {
            HistoryId = h.HistoryId;
            SoundId = h.SoundId;
            PlayedAt = h.PlayedAt;
            PlayDuration = h.PlayDuration;
            CompletedPlay = h.CompletedPlay;
            Sound = new SoundDto(h.Sound);
        }
    }
}
