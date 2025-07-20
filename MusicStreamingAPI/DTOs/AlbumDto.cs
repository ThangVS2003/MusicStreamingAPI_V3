using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.DTOs
{
    public class AlbumDto
    {
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; }
        public string ArtistName { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public string? Genre { get; set; }
        public int? TotalTracks { get; set; }
        public int? Duration { get; set; }
        public DateTime? CreatedAt { get; set; }
        public AlbumDto(Album a)
        {
            AlbumId = a.AlbumId;
            AlbumTitle = a.AlbumTitle;
            ArtistName = a.ArtistName;
            CoverImageUrl = a.CoverImageUrl;
            ReleaseDate = a.ReleaseDate;
            Genre = a.Genre;
            TotalTracks = a.TotalTracks;
            Duration = a.Duration;
            CreatedAt = a.CreatedAt;
        }
    }
}
