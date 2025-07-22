using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using MusicStreamingAPI.Models;
using MusicStreamingAPI.DTOs;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistsController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;

        public PlaylistsController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        // Tạo playlist mới
        [HttpPost]
        public async Task<IActionResult> CreatePlaylist([FromBody] CreatePlaylistRequest request)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(request.PlaylistName))
                {
                    return BadRequest(new { Message = "Tên playlist không được để trống." });
                }

                if (request.PlaylistName.Length > 200)
                {
                    return BadRequest(new { Message = "Tên playlist không được vượt quá 200 ký tự." });
                }

                if (request.Description != null && request.Description.Length > 1000)
                {
                    return BadRequest(new { Message = "Mô tả playlist không được vượt quá 1000 ký tự." });
                }

                if (request.CoverImageUrl != null && request.CoverImageUrl.Length > 500)
                {
                    return BadRequest(new { Message = "URL hình ảnh bìa không được vượt quá 500 ký tự." });
                }

                // Kiểm tra UserId tồn tại và đang hoạt động
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UserId);
                if (user == null || user.IsActive != true)
                {
                    return BadRequest(new { Message = "Người dùng không tồn tại hoặc không hoạt động." });
                }

                // Tạo playlist mới
                var playlist = new Playlist
                {
                    PlaylistName = request.PlaylistName,
                    Description = request.Description,
                    UserId = request.UserId,
                    CoverImageUrl = request.CoverImageUrl,
                    IsPublic = request.IsPublic,
                    TotalTracks = 0,
                    TotalDuration = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Playlists.Add(playlist);
                await _context.SaveChangesAsync();

                // Trả về thông tin playlist vừa tạo
                var response = new PlaylistResponse
                {
                    PlaylistId = playlist.PlaylistId,
                    PlaylistName = playlist.PlaylistName,
                    Description = playlist.Description,
                    UserId = playlist.UserId,
                    CoverImageUrl = playlist.CoverImageUrl,
                    IsPublic = playlist.IsPublic,
                    TotalTracks = playlist.TotalTracks,
                    TotalDuration = playlist.TotalDuration,
                    CreatedAt = playlist.CreatedAt,
                    UpdatedAt = playlist.UpdatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi tạo playlist.", Error = ex.Message });
            }
        }

        // Thêm bài nhạc vào playlist
        [HttpPost("{playlistId}/tracks")]
        public async Task<IActionResult> AddTrackToPlaylist([FromBody] AddTrackToPlaylistRequest request)
        {
            try
            {
                // Kiểm tra PlaylistId tồn tại
                var playlist = await _context.Playlists.FindAsync(request.PlaylistId);
                if (playlist == null)
                {
                    return BadRequest(new { Message = "Playlist không tồn tại." });
                }

                // Kiểm tra SoundId tồn tại và đang hoạt động
                var sound = await _context.Sounds.FirstOrDefaultAsync(s => s.SoundId == request.SoundId);
                if (sound == null || sound.IsActive != true)
                {
                    return BadRequest(new { Message = "Bài nhạc không tồn tại hoặc không hoạt động." });
                }

                // Kiểm tra bài nhạc đã có trong playlist chưa
                var trackExists = await _context.PlaylistTracks.AnyAsync(pt => pt.PlaylistId == request.PlaylistId && pt.SoundId == request.SoundId);
                if (trackExists)
                {
                    return BadRequest(new { Message = "Bài nhạc đã có trong playlist." });
                }

                // Tính TrackOrder tự động
                var maxTrackOrder = await _context.PlaylistTracks
                    .Where(pt => pt.PlaylistId == request.PlaylistId)
                    .MaxAsync(pt => (int?)pt.TrackOrder) ?? 0;

                // Thêm bài nhạc vào PlaylistTracks
                var playlistTrack = new PlaylistTrack
                {
                    PlaylistId = request.PlaylistId,
                    SoundId = request.SoundId,
                    TrackOrder = maxTrackOrder + 1,
                    AddedAt = DateTime.Now
                };

                _context.PlaylistTracks.Add(playlistTrack);
                await _context.SaveChangesAsync(); // Lưu PlaylistTrack trước

                // Cập nhật TotalTracks và TotalDuration
                playlist.TotalTracks = await _context.PlaylistTracks
                    .CountAsync(pt => pt.PlaylistId == request.PlaylistId);

                playlist.TotalDuration = await _context.PlaylistTracks
                    .Where(pt => pt.PlaylistId == request.PlaylistId)
                    .Join(_context.Sounds, pt => pt.SoundId, s => s.SoundId, (pt, s) => s.Duration)
                    .SumAsync();

                playlist.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync(); // Lưu cập nhật playlist

                return Ok(new
                {
                    Message = "Thêm bài nhạc vào playlist thành công.",
                    TotalTracks = playlist.TotalTracks,
                    TotalDuration = playlist.TotalDuration,
                    TrackOrder = playlistTrack.TrackOrder
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi thêm bài nhạc vào playlist.", Error = ex.Message });
            }
        }

        // Xóa bài nhạc khỏi playlist
        [HttpDelete("{playlistId}/tracks")]
        public async Task<IActionResult> RemoveTrackFromPlaylist([FromBody] RemoveTrackFromPlaylistRequest request)
        {
            try
            {
                // Kiểm tra PlaylistId tồn tại
                var playlist = await _context.Playlists.FindAsync(request.PlaylistId);
                if (playlist == null)
                {
                    return BadRequest(new { Message = "Playlist không tồn tại." });
                }

                // Kiểm tra bài nhạc có trong playlist không
                var playlistTrack = await _context.PlaylistTracks
                    .FirstOrDefaultAsync(pt => pt.PlaylistId == request.PlaylistId && pt.SoundId == request.SoundId);
                if (playlistTrack == null)
                {
                    return BadRequest(new { Message = "Bài nhạc không có trong playlist." });
                }

                // Xóa bài nhạc khỏi PlaylistTracks
                _context.PlaylistTracks.Remove(playlistTrack);
                await _context.SaveChangesAsync(); // Lưu xóa PlaylistTrack

                // Cập nhật TotalTracks và TotalDuration
                playlist.TotalTracks = await _context.PlaylistTracks
                    .CountAsync(pt => pt.PlaylistId == request.PlaylistId);

                playlist.TotalDuration = await _context.PlaylistTracks
                    .Where(pt => pt.PlaylistId == request.PlaylistId)
                    .Join(_context.Sounds, pt => pt.SoundId, s => s.SoundId, (pt, s) => s.Duration)
                    .SumAsync();

                playlist.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync(); // Lưu cập nhật playlist

                return Ok(new
                {
                    Message = "Xóa bài nhạc khỏi playlist thành công.",
                    TotalTracks = playlist.TotalTracks,
                    TotalDuration = playlist.TotalDuration
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xóa bài nhạc khỏi playlist.", Error = ex.Message });
            }
        }

        // Xóa playlist
        [HttpDelete("{playlistId}")]
        public async Task<IActionResult> DeletePlaylist(int playlistId)
        {
            try
            {
                // Kiểm tra PlaylistId tồn tại
                var playlist = await _context.Playlists.FindAsync(playlistId);
                if (playlist == null)
                {
                    return BadRequest(new { Message = "Playlist không tồn tại." });
                }

                // Xóa playlist (các bản ghi trong PlaylistTracks sẽ tự động bị xóa do ON DELETE CASCADE)
                _context.Playlists.Remove(playlist);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Xóa playlist thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xóa playlist.", Error = ex.Message });
            }
        }

        // Lấy danh sách playlist theo UserId
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetPlaylistsByUser(int userId)
        {
            try
            {
                // Kiểm tra UserId tồn tại và đang hoạt động
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
                if (user == null || user.IsActive != true)
                {
                    return BadRequest(new { Message = "Người dùng không tồn tại hoặc không hoạt động." });
                }

                // Lấy danh sách playlist
                var playlists = await _context.Playlists
                    .Where(p => p.UserId == userId)
                    .Select(p => new PlaylistResponse
                    {
                        PlaylistId = p.PlaylistId,
                        PlaylistName = p.PlaylistName,
                        Description = p.Description,
                        UserId = p.UserId,
                        CoverImageUrl = p.CoverImageUrl,
                        IsPublic = p.IsPublic,
                        TotalTracks = p.TotalTracks,
                        TotalDuration = p.TotalDuration,
                        CreatedAt = p.CreatedAt,
                        UpdatedAt = p.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(playlists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy danh sách playlist.", Error = ex.Message });
            }
        }
        // GET: api/playlists/{playlistId}/tracks
        [HttpGet("{playlistId}/tracks")]
        public async Task<IActionResult> GetSoundsByPlaylist(int playlistId)
        {
            try
            {
                // Kiểm tra playlist có tồn tại không
                var playlist = await _context.Playlists.FindAsync(playlistId);
                if (playlist == null)
                {
                    return NotFound(new { Message = "Playlist không tồn tại." });
                }

                // Lấy danh sách bài nhạc thuộc playlist
                var tracks = await _context.PlaylistTracks
                    .Where(pt => pt.PlaylistId == playlistId)
                    .Join(_context.Sounds,
                          pt => pt.SoundId,
                          s => s.SoundId,
                          (pt, s) => new TrackInPlaylistResponse
                          {
                              SoundId=s.SoundId,
                              Title = s.Title,
                              ArtistName = s.ArtistName,
                              CoverImageUrl= s.CoverImageUrl // hoặc s.Description nếu có
                          })
                    .ToListAsync();

                return Ok(tracks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy danh sách bài nhạc.", Error = ex.Message });
            }
        }

    }
}