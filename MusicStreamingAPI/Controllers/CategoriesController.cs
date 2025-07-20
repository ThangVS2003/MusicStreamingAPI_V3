using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreamingAPI.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using MusicStreamingAPI.DTOs;

namespace MusicStreamingAPI.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        public CategoriesController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.Categories.OrderBy(c => c.CategoryName).Select(c => new CategoryDto(c)).ToListAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Get trending categories (by most played sounds in last 7 days)
        /// </summary>
        [HttpGet("trending")]
        public async Task<IActionResult> GetTrending()
        {
            var weekAgo = DateTime.UtcNow.AddDays(-7);
            var trending = await _context.Sounds
                .Where(s => s.CategoryId != null && s.CreatedAt >= weekAgo)
                .GroupBy(s => s.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    PlayCount = g.Sum(s => s.PlayCount ?? 0)
                })
                .OrderByDescending(g => g.PlayCount)
                .Take(5)
                .ToListAsync();
            var categoryIds = trending.Select(t => t.CategoryId).ToList();
            var categories = await _context.Categories.Where(c => categoryIds.Contains(c.CategoryId)).Select(c => new CategoryDto(c)).ToListAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Get sounds by category
        /// </summary>
        [HttpGet("{id}/sounds")]
        public async Task<IActionResult> GetSoundsByCategory(int id)
        {
            var sounds = await _context.Sounds
                .Where(s => s.CategoryId == id)
                .OrderByDescending(s => s.CreatedAt)
                .Select(s => new SoundDto(s))
                .ToListAsync();
            return Ok(sounds);
        }
    }

  
} 