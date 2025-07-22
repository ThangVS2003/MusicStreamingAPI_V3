namespace MusicStreamingAPI.DTOs
{
    public class AddSoundToPlaylistResponseDto
    {

        public int PlaylistTrackId { get; set; }
        public int PlaylistId { get; set; }
        public int SoundId { get; set; }
        public int TrackOrder { get; set; }
        public DateTime AddedAt { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
