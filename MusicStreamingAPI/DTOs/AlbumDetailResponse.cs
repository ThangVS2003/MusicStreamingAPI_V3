﻿namespace MusicStreamingAPI.DTOs
{
    public class AlbumDetailResponse
    {
        public int AlbumId { get; set; }
        public string AlbumTitle { get; set; } = string.Empty;
        public string ArtistName { get; set; } = string.Empty;
        public string? CoverImageUrl { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public string? Genre { get; set; }
        public int? TotalTracks { get; set; }
        public int? Duration { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<SoundResponse> Tracks { get; set; } = new List<SoundResponse>();
    }
}