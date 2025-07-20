using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreamingAPI.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MusicStreamingAPI.DTOs;

namespace MusicStreamingAPI.Controllers
{
    [ApiController]
    [Route("api/likes")]
    [Authorize]
    public class LikesController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        public LikesController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Like or unlike a track (toggle)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> LikeOrUnlike([FromBody] LikeRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var liked = await _context.LikedTracks.FirstOrDefaultAsync(l => l.UserId == userId && l.SoundId == request.SoundId);
            if (liked != null)
            {
                _context.LikedTracks.Remove(liked);
                await _context.SaveChangesAsync();
                return Ok(new { liked = false });
            }
            else
            {
                var like = new LikedTrack { UserId = userId, SoundId = request.SoundId, LikedAt = DateTime.UtcNow };
                _context.LikedTracks.Add(like);
                await _context.SaveChangesAsync();
                return Ok(new { liked = true });
            }
        }
    }

    [ApiController]
    [Route("api/users/{id}/likes")]
    public class UserLikesController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        public UserLikesController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get liked tracks by user ID
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetLikedTracks(int id)
        {
            var likedTracks = await _context.LikedTracks
                .Where(l => l.UserId == id)
                .Include(l => l.Sound)
                .OrderByDescending(l => l.LikedAt)
                .ToListAsync();
            return Ok(likedTracks.Select(l => new LikedTrackDto(l)));
        }
    }

} 