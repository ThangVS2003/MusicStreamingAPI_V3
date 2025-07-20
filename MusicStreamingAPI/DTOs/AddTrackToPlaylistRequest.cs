namespace MusicStreamingAPI.DTOs
{
    public class AddTrackToPlaylistRequest
    {
        public int PlaylistId { get; set; }
        public int SoundId { get; set; }
    }
}