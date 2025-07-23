namespace MusicStreamingAPI.DTOs
{
    public class UpdateUserRequestDto
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? Country { get; set; }
        public string? Bio { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAdmin { get; set; }
    }
}
