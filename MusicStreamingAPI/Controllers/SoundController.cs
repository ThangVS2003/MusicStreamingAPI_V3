using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStreamingAPI.DTOs;
using MusicStreamingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SoundController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        private readonly ILogger<SoundController> _logger;

        public SoundController(MusicStreamingDbContext context, ILogger<SoundController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Sound/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSoundById(int id)
        {
            var sound = await _context.Sounds.FindAsync(id);

            if (sound == null || sound.IsActive == false || sound.IsPublic == false)
            {
                return NotFound(new { Message = "Bài nhạc không tồn tại hoặc đã bị ẩn." });
            }

            var uploader = await _context.Users.FindAsync(sound.UploadedBy);

            var soundDto = new SoundDto(sound)
            {
                UploaderName = (uploader != null) ? $"{uploader.FirstName} {uploader.LastName}" : "Unknown"
            };

            return Ok(soundDto);
        }

        // GET: api/Sound
        [HttpGet]
        public async Task<IActionResult> GetAllSound()
        {
            try
            {
                var sounds = await _context.Sounds
                    .Where(s => s.IsActive == true && s.IsPublic == true)
                    .ToListAsync();

                if (sounds == null || !sounds.Any())
                {
                    return NotFound(new { Message = "Không tìm thấy bài nhạc nào." });
                }

                var soundDtos = new List<SoundDto>();
                foreach (var sound in sounds)
                {
                    var uploader = await _context.Users.FindAsync(sound.UploadedBy);
                    var soundDto = new SoundDto(sound)
                    {
                        UploaderName = (uploader != null) ? $"{uploader.FirstName} {uploader.LastName}" : "Unknown"
                    };
                    soundDtos.Add(soundDto);
                }

                return Ok(soundDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách bài nhạc.");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy danh sách bài nhạc." });
            }
        }
        [HttpPost("available-for-playlist")]
        public async Task<ActionResult<List<SoundResponseDto>>> GetAvailableSounds(GetAvailableSoundsRequestDto request)
        {
            // Lấy UserId từ JWT token
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return Unauthorized(new
                {
                    Message = "Không thể xác thực người dùng",
                    Claims = User.Claims.Select(c => new { c.Type, c.Value })
                });
            }

            // Kiểm tra playlist tồn tại và thuộc về user
            var playlist = await _context.Playlists
                .Where(p => p.PlaylistId == request.PlaylistId && p.UserId == userId)
                .FirstOrDefaultAsync();
            if (playlist == null)
            {
                return NotFound(new { Message = "Playlist không tồn tại hoặc bạn không có quyền truy cập" });
            }

            // Lấy danh sách sounds không có trong playlist
            var availableSounds = await _context.Sounds
                .Where(s => s.IsActive == true) // Chỉ lấy bài hát đang hoạt động
                .Where(s => !_context.PlaylistTracks
                    .Where(pt => pt.PlaylistId == request.PlaylistId)
                    .Select(pt => pt.SoundId)
                    .Contains(s.SoundId))
                .Select(s => new SoundResponseDto
                {
                    SoundId = s.SoundId,
                    Title = s.Title,
                    ArtistName = s.ArtistName,
                    AlbumId = s.AlbumId,
                    CategoryId = s.CategoryId,
                    Duration = s.Duration,
                    FileUrl = s.FileUrl,
                    CoverImageUrl = s.CoverImageUrl,
                    Lyrics = s.Lyrics,
                    PlayCount = s.PlayCount,
                    LikeCount = s.LikeCount,
                    IsPublic = s.IsPublic,
                    IsActive = s.IsActive,
                    UploadedBy = s.UploadedBy,
                    CreatedAt = s.CreatedAt
                })
                .ToListAsync();

            return Ok(availableSounds);
        }
    }
}