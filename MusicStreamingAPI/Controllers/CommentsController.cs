using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreamingAPI.Models;
using MusicStreamingAPI.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;

        public CommentsController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        // Tạo bình luận mới
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(request.CommentText))
                    return BadRequest(new { Message = "Nội dung bình luận không được để trống." });
                if (request.CommentText.Length > 1000)
                    return BadRequest(new { Message = "Nội dung bình luận không được vượt quá 1000 ký tự." });

                // Kiểm tra UserId
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId && u.IsActive == true);
                if (user == null)
                    return BadRequest(new { Message = "Người dùng không tồn tại hoặc không hoạt động." });

                // Kiểm tra SoundId
                var sound = await _context.Sounds.FirstOrDefaultAsync(s => s.SoundId == request.SoundId && s.IsActive == true);
                if (sound == null)
                    return BadRequest(new { Message = "Bài nhạc không tồn tại hoặc không hoạt động." });

                // Kiểm tra ParentCommentId (nếu có)
                if (request.ParentCommentId.HasValue)
                {
                    var parentComment = await _context.Comments.FindAsync(request.ParentCommentId.Value);
                    if (parentComment == null || parentComment.SoundId != request.SoundId)
                        return BadRequest(new { Message = "Bình luận cha không tồn tại hoặc không thuộc bài nhạc này." });
                }

                // Tạo bình luận mới
                var comment = new Comment
                {
                    SoundId = request.SoundId,
                    UserId = request.UserId,
                    CommentText = request.CommentText,
                    ParentCommentId = request.ParentCommentId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();

                // Trả về thông tin bình luận
                var response = new CommentResponse
                {
                    CommentId = comment.CommentId,
                    SoundId = comment.SoundId,
                    UserId = comment.UserId,
                    Username = user.Username,
                    CommentText = comment.CommentText,
                    ParentCommentId = comment.ParentCommentId,
                    CreatedAt = comment.CreatedAt,
                    UpdatedAt = comment.UpdatedAt,
                    ChildComments = new List<CommentResponse>()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi tạo bình luận.", Error = ex.Message });
            }
        }

        // Lấy danh sách bình luận của bài nhạc
        [HttpGet("sound/{soundId}")]
        public async Task<IActionResult> GetCommentsBySound(int soundId)
        {
            try
            {
                // Kiểm tra SoundId
                var sound = await _context.Sounds.FirstOrDefaultAsync(s => s.SoundId == soundId && s.IsActive == true);
                if (sound == null)
                    return BadRequest(new { Message = "Bài nhạc không tồn tại hoặc không hoạt động." });

                // Lấy danh sách bình luận cấp cao nhất (ParentCommentId = null)
                var comments = await _context.Comments
                    .Where(c => c.SoundId == soundId && c.ParentCommentId == null)
                    .Include(c => c.User)
                    .Select(c => new CommentResponse
                    {
                        CommentId = c.CommentId,
                        SoundId = c.SoundId,
                        UserId = c.UserId,
                        Username = c.User.Username,
                        CommentText = c.CommentText,
                        ParentCommentId = c.ParentCommentId,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        ChildComments = _context.Comments
                            .Where(cc => cc.ParentCommentId == c.CommentId)
                            .Include(cc => cc.User)
                            .Select(cc => new CommentResponse
                            {
                                CommentId = cc.CommentId,
                                SoundId = cc.SoundId,
                                UserId = cc.UserId,
                                Username = cc.User.Username,
                                CommentText = cc.CommentText,
                                ParentCommentId = cc.ParentCommentId,
                                CreatedAt = cc.CreatedAt,
                                UpdatedAt = cc.UpdatedAt,
                                ChildComments = new List<CommentResponse>()
                            }).ToList()
                    })
                    .ToListAsync();

                return Ok(comments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy danh sách bình luận.", Error = ex.Message });
            }
        }

        // Cập nhật bình luận
        [HttpPut("{commentId}")]
        public async Task<IActionResult> UpdateComment(int commentId, [FromBody] UpdateCommentRequest request)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(request.CommentText))
                    return BadRequest(new { Message = "Nội dung bình luận không được để trống." });
                if (request.CommentText.Length > 1000)
                    return BadRequest(new { Message = "Nội dung bình luận không được vượt quá 1000 ký tự." });

                // Kiểm tra CommentId
                var comment = await _context.Comments
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.CommentId == commentId);
                if (comment == null)
                    return NotFound(new { Message = "Bình luận không tồn tại." });

                // Cập nhật bình luận
                comment.CommentText = request.CommentText;
                comment.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                // Trả về thông tin bình luận đã cập nhật
                var response = new CommentResponse
                {
                    CommentId = comment.CommentId,
                    SoundId = comment.SoundId,
                    UserId = comment.UserId,
                    Username = comment.User.Username,
                    CommentText = comment.CommentText,
                    ParentCommentId = comment.ParentCommentId,
                    CreatedAt = comment.CreatedAt,
                    UpdatedAt = comment.UpdatedAt,
                    ChildComments = new List<CommentResponse>()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi cập nhật bình luận.", Error = ex.Message });
            }
        }

        // Xóa bình luận
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            try
            {
                var comment = await _context.Comments.FindAsync(commentId);
                if (comment == null)
                    return NotFound(new { Message = "Bình luận không tồn tại." });

                _context.Comments.Remove(comment);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Xóa bình luận thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xóa bình luận.", Error = ex.Message });
            }
        }
    }
}