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
    [Route("api/history")]
    [Authorize]
    public class HistoryController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        public HistoryController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Add play history (for authenticated user)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddHistory([FromBody] AddHistoryRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            // Tìm bản ghi đã tồn tại theo SoundId
            var existingHistory = await _context.ListeningHistories
                .FirstOrDefaultAsync(h => h.UserId == userId && h.SoundId == request.SoundId);

            if (existingHistory != null)
            {
                // Cập nhật thời gian phát gần nhất và trạng thái
                existingHistory.PlayedAt = DateTime.UtcNow;
                existingHistory.PlayDuration = request.PlayDuration;
                existingHistory.CompletedPlay = request.CompletedPlay;

                await _context.SaveChangesAsync();
                return Ok(new ListeningHistoryDto(existingHistory));
            }

            // Nếu chưa có thì thêm mới
            var newHistory = new ListeningHistory
            {
                UserId = userId,
                SoundId = request.SoundId,
                PlayDuration = request.PlayDuration,
                CompletedPlay = request.CompletedPlay,
                PlayedAt = DateTime.UtcNow
            };
            _context.ListeningHistories.Add(newHistory);
            await _context.SaveChangesAsync();

            return Ok(new ListeningHistoryDto(newHistory));
        }

    }

    [ApiController]
    [Route("api/users/{id}/history")]
    public class UserHistoryController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;

        public UserHistoryController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get user's listening history
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetHistory(int id)
        {
            // Optional: Add authorization check if 'id' must match authenticated user's ID
            // var authenticatedUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            // if (authenticatedUserId != id)
            // {
            //     return Forbid(); // Or Unauthorized() if no specific authorization handler
            // }

            var history = await _context.ListeningHistories
                .Where(h => h.UserId == id)
                .Include(h => h.Sound)
                .OrderByDescending(h => h.PlayedAt)
                .ToListAsync();

            if (!history.Any())
            {
                return NotFound("No listening history found for this user.");
            }

            return Ok(history.Select(h => new ListeningHistoryDto(h)));
        }

        [HttpPost("delete-multiple")]
        public IActionResult DeleteMultiple([FromBody] List<int> historyIds)
        {
            var histories = _context.ListeningHistories.Where(h => historyIds.Contains(h.HistoryId)).ToList();
            _context.ListeningHistories.RemoveRange(histories);
            _context.SaveChanges();
            return Ok();
        }

    }



} 