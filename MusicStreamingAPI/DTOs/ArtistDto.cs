using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.DTOs
{
    public class ArtistDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public ArtistDto(User u)
        {
            UserId = u.UserId;
            Username = u.Username;
            FirstName = u.FirstName;
            LastName = u.LastName;
            AvatarUrl = u.AvatarUrl;
            Bio = u.Bio;
        }
    }
}
