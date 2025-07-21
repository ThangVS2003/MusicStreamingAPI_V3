using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MusicStreamingAPI.DTOs;
using MusicStreamingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}