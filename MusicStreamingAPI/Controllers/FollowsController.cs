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
    [Route("api/follows")]
    [Authorize]
    public class FollowsController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        public FollowsController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Follow or unfollow a user (toggle)
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> FollowOrUnfollow([FromBody] FollowRequest request)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            if (userId == request.FollowingId) return BadRequest("Cannot follow yourself");
            var follow = await _context.UserFollows.FirstOrDefaultAsync(f => f.FollowerId == userId && f.FollowingId == request.FollowingId);
            if (follow != null)
            {
                _context.UserFollows.Remove(follow);
                await _context.SaveChangesAsync();
                return Ok(new { following = false });
            }
            else
            {
                var newFollow = new UserFollow { FollowerId = userId, FollowingId = request.FollowingId, FollowedAt = DateTime.UtcNow };
                _context.UserFollows.Add(newFollow);
                await _context.SaveChangesAsync();
                return Ok(new { following = true });
            }
        }
    }

    [ApiController]
    [Route("api/users/{id}/following")]
    public class UserFollowingController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        public UserFollowingController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of users I'm following
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFollowing(int id)
        {
            var following = await _context.UserFollows
                .Where(f => f.FollowerId == id)
                .Include(f => f.Following)
                .OrderByDescending(f => f.FollowedAt)
                .ToListAsync();
            return Ok(following.Select(f => new UserFollowDto(f.Following)));
        }
    }

    [ApiController]
    [Route("api/users/{id}/followers")]
    public class UserFollowersController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        public UserFollowersController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get list of followers
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetFollowers(int id)
        {
            var followers = await _context.UserFollows
                .Where(f => f.FollowingId == id)
                .Include(f => f.Follower)
                .OrderByDescending(f => f.FollowedAt)
                .ToListAsync();
            return Ok(followers.Select(f => new UserFollowDto(f.Follower)));
        }
    }

   
} 