namespace MusicStreamingAPI.DTOs
{
    public class UpdateSoundRequest
    {
        public string? SoundName { get; set; }
        public int? CategoryId { get; set; }
        public int? AlbumId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsPublic { get; set; }
    }

}
