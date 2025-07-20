using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreamingAPI.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MusicStreamingAPI.DTOs;

namespace MusicStreamingAPI.Controllers
{
    [ApiController]
    [Route("api/library")]
    [Authorize]
    public class LibraryController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LibraryController(MusicStreamingDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        /// <summary>
        /// Get all tracks uploaded by the authenticated user
        /// </summary>
        [HttpGet("my-tracks")]
        public async Task<IActionResult> GetMyTracks()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var tracks = await _context.Sounds.Where(s => s.UploadedBy == userId).ToListAsync();
            return Ok(tracks.Select(s => new SoundDto(s)));
        }
    }
   
} 