using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreamingAPI.Models;
using MusicStreamingAPI.DTOs;

using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MusicStreamingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UsersController(MusicStreamingDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        /// <summary>
        /// Get user profile by user ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return Ok(new UserProfileDto(user));
        }

        /// <summary>
        /// Update user profile (protected)
        /// </summary>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UpdateProfileRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            if (userId != id) return Forbid();
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Bio = request.Bio;
            user.Country = request.Country;
            user.Gender = request.Gender;
            user.DateOfBirth = request.DateOfBirth;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(new UserProfileDto(user));
        }

        /// <summary>
        /// Upload avatar (protected)
        /// </summary>
        //[Authorize]
        //[HttpPost("{id}/avatar")]
        //public async Task<IActionResult> UploadAvatar(int id, [FromForm] IFormFile avatar)
        //{
        //    var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        //    if (userId != id) return Forbid();
        //    var user = await _context.Users.FindAsync(id);
        //    if (user == null) return NotFound();
        //    if (avatar == null || avatar.Length == 0) return BadRequest("No file uploaded");
        //    var ext = Path.GetExtension(avatar.FileName);
        //    var fileName = $"avatar_{id}_{Guid.NewGuid()}{ext}";
        //    var avatarDir = Path.Combine(_env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "avatars");
        //    if (!Directory.Exists(avatarDir)) Directory.CreateDirectory(avatarDir);
        //    var filePath = Path.Combine(avatarDir, fileName);
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await avatar.CopyToAsync(stream);
        //    }
        //    user.AvatarUrl = $"/avatars/{fileName}";
        //    user.UpdatedAt = DateTime.UtcNow;
        //    await _context.SaveChangesAsync();
        //    return Ok(new { avatarUrl = user.AvatarUrl });
        //}
    }

   
} 