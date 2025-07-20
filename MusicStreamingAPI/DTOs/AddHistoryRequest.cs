using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.DTOs
{
    // DTOs
    public class AddHistoryRequest
    {
        public int SoundId { get; set; }
        public int? PlayDuration { get; set; }
        public bool? CompletedPlay { get; set; }
    }
    
    
}
