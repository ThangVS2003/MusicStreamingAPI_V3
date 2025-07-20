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
            var history = new ListeningHistory
            {
                UserId = userId,
                SoundId = request.SoundId,
                PlayDuration = request.PlayDuration,
                CompletedPlay = request.CompletedPlay,
                PlayedAt = DateTime.UtcNow
            };
            _context.ListeningHistories.Add(history);
            await _context.SaveChangesAsync();
            return Ok(new ListeningHistoryDto(history));
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
            var history = await _context.ListeningHistories
                .Where(h => h.UserId == id)
                .Include(h => h.Sound)
                .OrderByDescending(h => h.PlayedAt)
                .ToListAsync();
            return Ok(history.Select(h => new ListeningHistoryDto(h)));
        }
    }

    
} 