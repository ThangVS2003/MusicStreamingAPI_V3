using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.DTOs
{
    // DTOs
    public class FollowRequest
    {
        public int FollowingId { get; set; }
    }
    public class UserFollowDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public UserFollowDto(User user)
        {
            UserId = user.UserId;
            Username = user.Username;
            AvatarUrl = user.AvatarUrl;
            Bio = user.Bio;
        }
    }
}
