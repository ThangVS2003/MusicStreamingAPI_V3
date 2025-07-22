namespace MusicStreamingAPI.DTOs
{
    public class RemoveSoundFromPlaylistResponseDto
    {
        public int PlaylistId { get; set; }
        public int SoundId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
