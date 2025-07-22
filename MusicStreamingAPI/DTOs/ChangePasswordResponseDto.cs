namespace MusicStreamingAPI.DTOs
{
    public class ChangePasswordResponseDto
    {
        public int UserId { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
