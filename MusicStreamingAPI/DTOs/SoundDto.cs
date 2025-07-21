using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.DTOs
{
    // DTOs
    public class SoundDto
    {
        public int SoundId { get; set; }
        public string Title { get; set; }
        public string ArtistName { get; set; }
        public int? AlbumId { get; set; }
        public int? CategoryId { get; set; }
        public int Duration { get; set; }
        public string FileUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public string? Lyrics { get; set; }
        public int? PlayCount { get; set; }
        public int? LikeCount { get; set; }
        public bool? IsPublic { get; set; }
        public bool? IsActive { get; set; }
        public int? UploadedBy { get; set; }
        public string? UploaderName { get; set; }
        public DateTime? CreatedAt { get; set; }
        public SoundDto(Sound s)
        {
            SoundId = s.SoundId;
            Title = s.Title;
            ArtistName = s.ArtistName;
            AlbumId = s.AlbumId;
            CategoryId = s.CategoryId;
            Duration = s.Duration;
            FileUrl = s.FileUrl;
            CoverImageUrl = s.CoverImageUrl;
            Lyrics = s.Lyrics;
            PlayCount = s.PlayCount;
            LikeCount = s.LikeCount;
            IsPublic = s.IsPublic;
            IsActive = s.IsActive;
            UploadedBy = s.UploadedBy;
            CreatedAt = s.CreatedAt;
            UploaderName = UploaderName;
        }
    }
}
