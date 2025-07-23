using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreamingAPI.Models;
using System;
using System.Threading.Tasks;
using MusicStreamingAPI.DTOs;
using System.Security.Claims;

namespace MusicStreamingAPI.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        public AdminController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    DateOfBirth = u.DateOfBirth,
                    Gender = u.Gender,
                    Country = u.Country,
                    AvatarUrl = u.AvatarUrl,
                    Bio = u.Bio,
                    IsActive = u.IsActive,
                    IsAdmin = u.IsAdmin,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    LastLoginAt = u.LastLoginAt
                })
                .ToListAsync();

            return Ok(users);
        }

        /// <summary>
        /// Get a user by ID (Admin only)
        /// </summary>
        [HttpGet("users/{id}")]
        public async Task<ActionResult<UserResponseDto>> GetUser(int id)
        {
            var user = await _context.Users
                .Select(u => new UserResponseDto
                {
                    UserId = u.UserId,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    DateOfBirth = u.DateOfBirth,
                    Gender = u.Gender,
                    Country = u.Country,
                    AvatarUrl = u.AvatarUrl,
                    Bio = u.Bio,
                    IsActive = u.IsActive,
                    IsAdmin = u.IsAdmin,
                    CreatedAt = u.CreatedAt,
                    UpdatedAt = u.UpdatedAt,
                    LastLoginAt = u.LastLoginAt
                })
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
            {
                return NotFound(new { Message = $"Người dùng với ID {id} không tồn tại" });
            }

            return Ok(user);
        }

        /// <summary>
        /// Create a new user (Admin only)
        /// </summary>
        [HttpPost("users")]
        public async Task<ActionResult<UserResponseDto>> CreateUser(CreateUserRequestDto request)
        {
            // Kiểm tra username và email đã tồn tại
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return BadRequest(new { Message = "Tên người dùng đã tồn tại" });
            }

            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest(new { Message = "Email đã tồn tại" });
            }

            // Kiểm tra mật khẩu
            if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6)
            {
                return BadRequest(new { Message = "Mật khẩu phải có ít nhất 6 ký tự" });
            }

            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = request.Password, // Lưu dạng văn bản thuần (như yêu cầu trước)
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Country = request.Country,
                Bio = request.Bio,
                IsActive = request.IsActive,
                IsAdmin = request.IsAdmin,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var response = new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Country = user.Country,
                Bio = user.Bio,
                IsActive = user.IsActive,
                IsAdmin = user.IsAdmin,
                CreatedAt = user.CreatedAt
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, response);
        }

        /// <summary>
        /// Update a user (Admin only)
        /// </summary>
        [HttpPut("users/{id}")]
        public async Task<ActionResult<UserResponseDto>> UpdateUser(int id, UpdateUserRequestDto request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"Người dùng với ID {id} không tồn tại" });
            }

            // Kiểm tra username và email (nếu thay đổi)
            if (!string.IsNullOrEmpty(request.Username) && request.Username != user.Username)
            {
                if (await _context.Users.AnyAsync(u => u.Username == request.Username))
                {
                    return BadRequest(new { Message = "Tên người dùng đã tồn tại" });
                }
                user.Username = request.Username;
            }

            if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
            {
                if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                {
                    return BadRequest(new { Message = "Email đã tồn tại" });
                }
                user.Email = request.Email;
            }

            // Cập nhật các trường khác
            if (request.FirstName != null) user.FirstName = request.FirstName;
            if (request.LastName != null) user.LastName = request.LastName;
            if (request.DateOfBirth.HasValue) user.DateOfBirth = request.DateOfBirth;
            if (request.Gender != null) user.Gender = request.Gender;
            if (request.Country != null) user.Country = request.Country;
            if (request.Bio != null) user.Bio = request.Bio;
            if (request.IsActive.HasValue) user.IsActive = request.IsActive;
            if (request.IsAdmin.HasValue) user.IsAdmin = request.IsAdmin;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                Country = user.Country,
                AvatarUrl = user.AvatarUrl,
                Bio = user.Bio,
                IsActive = user.IsActive,
                IsAdmin = user.IsAdmin,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                LastLoginAt = user.LastLoginAt
            };

            return Ok(response);
        }

        /// <summary>
        /// Delete a user (Admin only)
        /// </summary>
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = $"Người dùng với ID {id} không tồn tại" });
            }

            // Kiểm tra nếu admin đang cố xóa chính mình
            var adminIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(adminIdClaim, out int adminId) && adminId == id)
            {
                return BadRequest(new { Message = "Không thể xóa tài khoản admin đang đăng nhập" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Approve/Reject/Hide/Remove sounds
        /// </summary>
        [HttpPut("sounds/{id}/moderation")]
        public async Task<IActionResult> ModerateSound(int id, [FromBody] ModerateSoundRequest request)
        {
            var sound = await _context.Sounds.FindAsync(id);
            if (sound == null) return NotFound();
            switch (request.Action?.ToLower())
            {
                case "approve":
                    sound.IsActive = true;
                    break;
                case "reject":
                case "remove":
                    _context.Sounds.Remove(sound);
                    await _context.SaveChangesAsync();
                    return NoContent();
                case "hide":
                    sound.IsActive = false;
                    break;
                default:
                    return BadRequest("Invalid action");
            }
            await _context.SaveChangesAsync();
            return Ok(new SoundDto(sound));
        }
        [HttpGet("statistics")]
        public async Task<ActionResult<StatisticsResponseDto>> GetStatistics()
        {
            var statistics = new StatisticsResponseDto
            {
                CategoryCount = await _context.Categories.CountAsync(),
                SoundCount = await _context.Sounds.CountAsync(),
                UserCount = await _context.Users.CountAsync(),
                CommentCount = await _context.Comments.CountAsync()
            };

            return Ok(statistics);
        }
    }

  
} 