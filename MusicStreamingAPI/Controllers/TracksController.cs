using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreamingAPI.Models;
using MusicStreamingAPI.DTOs;

using System;
using System.Threading.Tasks;

namespace MusicStreamingAPI.Controllers
{
    [ApiController]
    [Route("api/sounds")]
    public class TracksController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        public TracksController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get full track info by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTrack(int id)
        {
            var sound = await _context.Sounds.Include(s => s.Album).Include(s => s.Category).FirstOrDefaultAsync(s => s.SoundId == id);
            if (sound == null) return NotFound();
            return Ok(new SoundDto(sound));
        }

        /// <summary>
        /// Increase play count when played
        /// </summary>
        [HttpPost("{id}/play")]
        public async Task<IActionResult> PlayTrack(int id)
        {
            var sound = await _context.Sounds.FindAsync(id);
            if (sound == null) return NotFound();
            sound.PlayCount = (sound.PlayCount ?? 0) + 1;
            await _context.SaveChangesAsync();
            return Ok(new { playCount = sound.PlayCount });
        }
    }

   
} 