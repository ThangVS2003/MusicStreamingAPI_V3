using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreamingAPI.DTOs;
using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LikeController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;

        public LikeController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> PostLike([FromBody] LikedTrackDTO dto)
        {
            // Validate UserId và SoundId
            var user = await _context.Users.FindAsync(dto.UserId);
            var sound = await _context.Sounds.FindAsync(dto.SoundId);

            if (user == null || sound == null)
                return NotFound("User or Sound not found.");

            // Kiểm tra đã like chưa
            var existing = await _context.LikedTracks
                .FirstOrDefaultAsync(l => l.UserId == dto.UserId && l.SoundId == dto.SoundId);

            if (existing != null)
                return Conflict("Track already liked.");

            // Tạo mới bản ghi LikedTrack
            var likedTrack = new LikedTrack
            {
                UserId = dto.UserId,
                SoundId = dto.SoundId,
                LikedAt = DateTime.UtcNow
            };

            _context.LikedTracks.Add(likedTrack);
            await _context.SaveChangesAsync();

            // Gán lại DTO để trả về
            dto.LikedAt = likedTrack.LikedAt;             // giả sử User có Username          // giả sử Sound có Title

            return Ok(dto);
        }
    }
}
