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
            try
            {
                var user = await _context.Users.FindAsync(dto.UserId);
                var sound = await _context.Sounds.FindAsync(dto.Sound.SoundId);

                if (user == null || sound == null)
                    return NotFound(new { Message = "Người dùng hoặc bài nhạc không tồn tại." });

                var existing = await _context.LikedTracks
                    .FirstOrDefaultAsync(l => l.UserId == dto.UserId && l.SoundId == dto.Sound.SoundId);

                if (existing != null)
                {
                    _context.LikedTracks.Remove(existing);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        Message = "Đã bỏ thích bài nhạc.",
                        SoundId = dto.Sound.SoundId,
                        UserId = dto.UserId,
                        IsLiked = false
                    });
                }
                else
                {
                    var likedTrack = new LikedTrack
                    {
                        UserId = dto.UserId,
                        SoundId = dto.Sound.SoundId,
                        LikedAt = DateTime.UtcNow
                    };

                    _context.LikedTracks.Add(likedTrack);
                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        Message = "Đã thêm vào danh sách yêu thích.",
                        SoundId = dto.Sound.SoundId,
                        UserId = dto.UserId,
                        IsLiked = true,
                        LikedAt = likedTrack.LikedAt
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi khi xử lý yêu thích.", Error = ex.Message });
            }
        }

    }
}
