using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.DTOs
{
    // DTOs
    public class UserAdminDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAdmin { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserAdminDto(User u)
        {
            UserId = u.UserId;
            Username = u.Username;
            Email = u.Email;
            IsActive = u.IsActive;
            IsAdmin = u.IsAdmin;
            CreatedAt = u.CreatedAt;
            UpdatedAt = u.UpdatedAt;
        }
    }
    public class UpdateUserStatusRequest
    {
        public bool IsActive { get; set; }
    }
    public class ModerateSoundRequest
    {
        public string Action { get; set; } // approve, reject, hide, remove
    }
   
}
