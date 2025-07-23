using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreamingAPI.Models;
using System;
using System.Threading.Tasks;
using MusicStreamingAPI.DTOs;

namespace MusicStreamingAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        public AdminController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users.OrderBy(u => u.UserId).Select(u => new UserAdminDto(u)).ToListAsync();
            return Ok(users);
        }

        /// <summary>
        /// Ban or activate user
        /// </summary>
        [HttpPut("users/{id}/status")]
        public async Task<IActionResult> UpdateUserStatus(int id, [FromBody] UpdateUserStatusRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.IsActive = request.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new UserAdminDto(user));
        }

        /// <summary>
        /// Approve/Reject/Hide/Remove sounds
        /// </summary>
        [HttpPut("sounds/{id}/moderation")]
        public async Task<IActionResult> ModerateSound(int id, [FromBody] ModerateSoundRequest request)
        {
            var sound = await _context.Sounds.FindAsync(id);
            if (sound == null) return NotFound();
            switch (request.Action?.ToLower())
            {
                case "approve":
                    sound.IsActive = true;
                    break;
                case "reject":
                case "remove":
                    _context.Sounds.Remove(sound);
                    await _context.SaveChangesAsync();
                    return NoContent();
                case "hide":
                    sound.IsActive = false;
                    break;
                default:
                    return BadRequest("Invalid action");
            }
            await _context.SaveChangesAsync();
            return Ok(new SoundDto(sound));
        }
        [HttpGet("AllSounds")]
        public async Task<ActionResult<IEnumerable<SoundAdminDto>>> GetAllSounds()
        {
            var sounds = await _context.Sounds
                .Include(s => s.UploadedByNavigation)
                .Include(s => s.Album)
                .Include(s => s.Category)
                .Select(s => new SoundAdminDto(s))
                .ToListAsync();

            return Ok(sounds);
        }
        [HttpGet("by-username")]
        public async Task<ActionResult<IEnumerable<SoundAdminDto>>> GetSoundsByUsername([FromQuery] string username)
        {
            var sounds = await _context.Sounds
                .Include(s => s.UploadedByNavigation)
                .Include(s => s.Album)
                .Include(s => s.Category)
                .Where(s => s.UploadedByNavigation.Username.ToLower().Contains(username.ToLower()))
                .Select(s => new SoundAdminDto(s))
                .ToListAsync();

            return Ok(sounds);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSound(int id, [FromBody] UpdateSoundRequest request)
        {
            var sound = await _context.Sounds.FindAsync(id);
            if (sound == null)
                return NotFound();

            sound.Title = request.SoundName ?? sound.Title;
            sound.CategoryId = request.CategoryId ?? sound.CategoryId;
            sound.AlbumId = request.AlbumId ?? sound.AlbumId;
            sound.IsActive = request.IsActive ?? sound.IsActive;
            sound.IsPublic = request.IsPublic ?? sound.IsPublic;

            sound.CreatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

    }

  
} 