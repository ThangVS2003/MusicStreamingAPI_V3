using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.DTOs
{
    // DTOs
    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Bio { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAdmin { get; set; }
        public UserProfileDto(User user)
        {
            UserId = user.UserId;
            Username = user.Username;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            DateOfBirth = user.DateOfBirth;
            Gender = user.Gender;
            Country = user.Country;
            AvatarUrl = user.AvatarUrl;
            Bio = user.Bio;
            IsActive = user.IsActive;
            IsAdmin = user.IsAdmin;
        }
    }
    public class UpdateProfileRequest
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? Bio { get; set; }
    }
}
