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
    }

  
} 