using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreamingAPI.Models;
using MusicStreamingAPI.DTOs;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SoundsController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;

        public SoundsController(MusicStreamingDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> UploadSound([FromBody] UploadSoundRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title) ||
                string.IsNullOrWhiteSpace(request.ArtistName) ||
                string.IsNullOrWhiteSpace(request.FileUrl))
            {
                return BadRequest("Title, ArtistName và FileUrl là bắt buộc.");
            }

            var driveFileId = ExtractGoogleDriveFileId(request.FileUrl);
            if (string.IsNullOrEmpty(driveFileId))
            {
                return BadRequest("FileUrl không đúng định dạng Google Drive.");
            }

            string directDownloadUrl = $"https://drive.google.com/uc?export=download&id={driveFileId}";

            var newSound = new Sound
            {
                Title = request.Title,
                ArtistName = request.ArtistName,
                FileUrl = directDownloadUrl,
                CoverImageUrl = request.CoverImageUrl,
                IsActive = false,
                IsPublic = true,
                CreatedAt = DateTime.UtcNow,
                UploadedBy = request.UserId
            };

            _context.Sounds.Add(newSound);
            await _context.SaveChangesAsync();


            return Ok(new { message = "Tải lên thành công", soundId = newSound.SoundId });
        }


        private string? ExtractGoogleDriveFileId(string url)
        {
            try
            {
                if (url.Contains("drive.google.com"))
                {
                    // Dạng 1: https://drive.google.com/file/d/FILE_ID/view?usp=sharing
                    var parts = url.Split('/');
                    var idIndex = Array.IndexOf(parts, "d");
                    if (idIndex != -1 && idIndex + 1 < parts.Length)
                    {
                        return parts[idIndex + 1];
                    }

                    // Dạng 2: https://drive.google.com/open?id=FILE_ID
                    var uri = new Uri(url);
                    var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                    return query["id"];
                }
            }
            catch
            {
                return null;
            }

            return null;
        }
    }


}
