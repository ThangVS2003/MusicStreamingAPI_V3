namespace MusicStreamingAPI.DTOs
{
    public class RemoveTrackFromPlaylistRequest
    {
        public int PlaylistId { get; set; }
        public int SoundId { get; set; }
    }
}