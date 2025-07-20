using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.DTOs
{
    // DTOs
    public class PlaylistDto
    {
        public int PlaylistId { get; set; }
        public string PlaylistName { get; set; }
        public string? Description { get; set; }
        public string? CoverImageUrl { get; set; }
        public int? TotalTracks { get; set; }
        public int? TotalDuration { get; set; }
        public bool? IsPublic { get; set; }
        public DateTime? CreatedAt { get; set; }
        public PlaylistDto(Playlist p)
        {
            PlaylistId = p.PlaylistId;
            PlaylistName = p.PlaylistName;
            Description = p.Description;
            CoverImageUrl = p.CoverImageUrl;
            TotalTracks = p.TotalTracks;
            TotalDuration = p.TotalDuration;
            IsPublic = p.IsPublic;
            CreatedAt = p.CreatedAt;
        }
    }
   
  
}
