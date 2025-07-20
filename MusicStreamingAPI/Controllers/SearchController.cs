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
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        public SearchController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Search by keyword and filter by type (sound, album, artist, category)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] string? type)
        {
            query = query?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required");
            type = type?.ToLower();
            var results = new SearchResultsDto();
            if (string.IsNullOrEmpty(type) || type == "sound")
            {
                results.Sounds = await _context.Sounds
                    .Where(s => s.Title.Contains(query) || s.ArtistName.Contains(query) || (s.Lyrics != null && s.Lyrics.Contains(query)))
                    .OrderByDescending(s => s.CreatedAt)
                    .Take(20)
                    .Select(s => new SoundDto(s))
                    .ToListAsync();
            }
            if (string.IsNullOrEmpty(type) || type == "album")
            {
                results.Albums = await _context.Albums
                    .Where(a => a.AlbumTitle.Contains(query) || a.ArtistName.Contains(query) || (a.Genre != null && a.Genre.Contains(query)))
                    .OrderByDescending(a => a.CreatedAt)
                    .Take(20)
                    .Select(a => new AlbumDto(a))
                    .ToListAsync();
            }
            if (string.IsNullOrEmpty(type) || type == "artist")
            {
                results.Artists = await _context.Users
                    .Where(u => u.Username.Contains(query) || (u.FirstName != null && u.FirstName.Contains(query)) || (u.LastName != null && u.LastName.Contains(query)))
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(20)
                    .Select(u => new ArtistDto(u))
                    .ToListAsync();
            }
            if (string.IsNullOrEmpty(type) || type == "category")
            {
                results.Categories = await _context.Categories
                    .Where(c => c.CategoryName.Contains(query) || (c.Description != null && c.Description.Contains(query)))
                    .OrderByDescending(c => c.CreatedAt)
                    .Take(20)
                    .Select(c => new CategoryDto(c))
                    .ToListAsync();
            }
            return Ok(results);
        }
    }

    
} 