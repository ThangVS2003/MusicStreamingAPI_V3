using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.DTOs
{
    public class SoundAdminDto
    {
        public int SoundId { get; set; }
        public string Title { get; set; } = null!;
        public string ArtistName { get; set; } = null!;
        public int? AlbumId { get; set; }
        public string? AlbumTitle { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int Duration { get; set; }
        public string FileUrl { get; set; } = null!;
        public string? CoverImageUrl { get; set; }
        public string? Lyrics { get; set; }
        public int? PlayCount { get; set; }
        public int? LikeCount { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsActive { get; set; }
        public int? UploadedBy { get; set; }
        public string? UploadedByUsername { get; set; }
        public DateTime? CreatedAt { get; set; }

        public SoundAdminDto(Sound s)
        {
            SoundId = s.SoundId;
            Title = s.Title;
            ArtistName = s.ArtistName;
            AlbumId = s.AlbumId;
            AlbumTitle = s.Album?.AlbumTitle;
            CategoryId = s.CategoryId;
            CategoryName = s.Category?.CategoryName;
            Duration = s.Duration;
            FileUrl = s.FileUrl;
            CoverImageUrl = s.CoverImageUrl;
            Lyrics = s.Lyrics;
            PlayCount = s.PlayCount;
            LikeCount = s.LikeCount;
            IsPublic = s.IsPublic;
            IsActive = s.IsActive;
            UploadedBy = s.UploadedBy;
            UploadedByUsername = s.UploadedByNavigation?.Username;
            CreatedAt = s.CreatedAt;
        }
    }
}
