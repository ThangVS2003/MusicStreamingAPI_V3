using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreamingAPI.DTOs;
using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlaylistTrackController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        public PlaylistTrackController(MusicStreamingDbContext context)
        {
            _context = context;
        }
        [HttpPost("add-sound")]
        public async Task<ActionResult<AddSoundToPlaylistResponseDto>> AddSoundToPlaylist(AddSoundToPlaylistResponseDto request)
        {
            // Kiểm tra playlist tồn tại
            var playlist = await _context.Playlists.FindAsync(request.PlaylistId);
            if (playlist == null)
            {
                return NotFound(new { Message = "Playlist không tồn tại" });
            }

            // Kiểm tra sound tồn tại
            var sound = await _context.Sounds.FindAsync(request.SoundId);
            if (sound == null)
            {
                return NotFound(new { Message = "Bài hát không tồn tại" });
            }

            // Kiểm tra xem bài hát đã có trong playlist chưa
            var existingTrack = await _context.PlaylistTracks
                .AnyAsync(pt => pt.PlaylistId == request.PlaylistId && pt.SoundId == request.SoundId);
            if (existingTrack)
            {
                return BadRequest(new { Message = "Bài hát đã tồn tại trong playlist" });
            }

            // Tính track order mới (lấy max TrackOrder hiện tại + 1)
            var maxOrder = await _context.PlaylistTracks
                .Where(pt => pt.PlaylistId == request.PlaylistId)
                .MaxAsync(pt => (int?)pt.TrackOrder) ?? 0;

            var playlistTrack = new PlaylistTrack
            {
                PlaylistId = request.PlaylistId,
                SoundId = request.SoundId,
                TrackOrder = maxOrder + 1,
                AddedAt = DateTime.UtcNow
            };

            _context.PlaylistTracks.Add(playlistTrack);

            // Cập nhật TotalTracks trong Playlist
            playlist.TotalTracks = (playlist.TotalTracks ?? 0) + 1;
            playlist.TotalDuration = (playlist.TotalDuration ?? 0) + sound.Duration;
            playlist.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = new AddSoundToPlaylistResponseDto
            {
                PlaylistTrackId = playlistTrack.PlaylistTrackId,
                PlaylistId = playlistTrack.PlaylistId,
                SoundId = playlistTrack.SoundId,
                TrackOrder = playlistTrack.TrackOrder,
                AddedAt = playlistTrack.AddedAt.Value,
                Message = "Thêm bài hát vào playlist thành công"
            };

            return Ok(response);
        }
        [HttpDelete("remove-sound")]
        public async Task<ActionResult<RemoveSoundFromPlaylistResponseDto>> RemoveSoundFromPlaylist(RemoveSoundFromPlaylistResponseDto request)
        {
            // Kiểm tra playlist tồn tại
            var playlist = await _context.Playlists.FindAsync(request.PlaylistId);
            if (playlist == null)
            {
                return NotFound(new { Message = "Playlist không tồn tại" });
            }

            // Kiểm tra sound tồn tại
            var sound = await _context.Sounds.FindAsync(request.SoundId);
            if (sound == null)
            {
                return NotFound(new { Message = "Bài hát không tồn tại" });
            }

            // Tìm track trong playlist
            var playlistTrack = await _context.PlaylistTracks
                .FirstOrDefaultAsync(pt => pt.PlaylistId == request.PlaylistId && pt.SoundId == request.SoundId);
            if (playlistTrack == null)
            {
                return NotFound(new { Message = "Bài hát không tồn tại trong playlist" });
            }

            // Xóa track khỏi playlist
            _context.PlaylistTracks.Remove(playlistTrack);

            // Cập nhật TotalTracks và TotalDuration trong Playlist
            playlist.TotalTracks = (playlist.TotalTracks ?? 1) - 1;
            playlist.TotalDuration = (playlist.TotalDuration ?? sound.Duration) - sound.Duration;
            playlist.UpdatedAt = DateTime.UtcNow;

            // Điều chỉnh TrackOrder của các track còn lại
            var tracksToUpdate = await _context.PlaylistTracks
                .Where(pt => pt.PlaylistId == request.PlaylistId && pt.TrackOrder > playlistTrack.TrackOrder)
                .ToListAsync();
            foreach (var track in tracksToUpdate)
            {
                track.TrackOrder -= 1;
            }

            await _context.SaveChangesAsync();

            var response = new RemoveSoundFromPlaylistResponseDto
            {
                PlaylistId = request.PlaylistId,
                SoundId = request.SoundId,
                Message = "Xóa bài hát khỏi playlist thành công"
            };

            return Ok(response);
        }
    }
}
