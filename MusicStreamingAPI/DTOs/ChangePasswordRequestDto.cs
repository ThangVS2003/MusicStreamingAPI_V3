﻿namespace MusicStreamingAPI.DTOs
{
    public class ChangePasswordRequestDto
    {
        public string OldPassword { get; set; } = string.Empty; 
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
