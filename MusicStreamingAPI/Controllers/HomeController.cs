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
    [Route("api/home")]
    public class HomeController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        public HomeController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get home screen data: featured playlists, albums, trending sounds, latest uploads, most played, top liked
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetHome()
        {
            var now = DateTime.UtcNow;
            var weekAgo = now.AddDays(-7);
            var featuredPlaylists = await _context.Playlists
                .OrderByDescending(p => p.CreatedAt)
                .Take(5)
                .Select(p => new PlaylistDto(p))
                .ToListAsync();
            var featuredAlbums = await _context.Albums
                .OrderByDescending(a => a.CreatedAt)
                .Take(5)
                .Select(a => new AlbumDto(a))
                .ToListAsync();
            var trendingSounds = await _context.Sounds
                .Where(s => s.CreatedAt >= weekAgo)
                .OrderByDescending(s => s.PlayCount)
                .Take(10)
                .Select(s => new SoundDto(s))
                .ToListAsync();
            var latestUploads = await _context.Sounds
                .OrderByDescending(s => s.CreatedAt)
                .Take(10)
                .Select(s => new SoundDto(s))
                .ToListAsync();
            var mostPlayed = await _context.Sounds
                .OrderByDescending(s => s.PlayCount)
                .Take(10)
                .Select(s => new SoundDto(s))
                .ToListAsync();
            var topLiked = await _context.Sounds
                .OrderByDescending(s => s.LikeCount)
                .Take(10)
                .Select(s => new SoundDto(s))
                .ToListAsync();
            return Ok(new
            {
                featuredPlaylists,
                featuredAlbums,
                trendingSounds,
                latestUploads,
                mostPlayed,
                topLiked
            });
        }
    }

    
} 