using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreamingAPI.Models;
using MusicStreamingAPI.DTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;

        public AlbumsController(MusicStreamingDbContext context)
        {
            _context = context;
        }

        // Tạo album mới
        [HttpPost]
        public async Task<IActionResult> CreateAlbum([FromBody] CreateAlbumRequest request)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(request.AlbumTitle))
                    return BadRequest(new { Message = "Tên album không được để trống." });
                if (request.AlbumTitle.Length > 200)
                    return BadRequest(new { Message = "Tên album không được vượt quá 200 ký tự." });
                if (string.IsNullOrWhiteSpace(request.ArtistName))
                    return BadRequest(new { Message = "Tên nghệ sĩ không được để trống." });
                if (request.ArtistName.Length > 100)
                    return BadRequest(new { Message = "Tên nghệ sĩ không được vượt quá 100 ký tự." });
                if (request.CoverImageUrl != null && request.CoverImageUrl.Length > 500)
                    return BadRequest(new { Message = "URL hình ảnh bìa không được vượt quá 500 ký tự." });
                if (request.Genre != null && request.Genre.Length > 50)
                    return BadRequest(new { Message = "Thể loại không được vượt quá 50 ký tự." });

                // Tạo album mới
                var album = new Album
                {
                    AlbumTitle = request.AlbumTitle,
                    ArtistName = request.ArtistName,
                    CoverImageUrl = request.CoverImageUrl,
                    ReleaseDate = request.ReleaseDate,
                    Genre = request.Genre,
                    TotalTracks = 0,
                    Duration = 0,
                    CreatedAt = DateTime.Now
                };

                _context.Albums.Add(album);
                await _context.SaveChangesAsync();

                // Trả về thông tin album vừa tạo
                var response = new AlbumResponse
                {
                    AlbumId = album.AlbumId,
                    AlbumTitle = album.AlbumTitle,
                    ArtistName = album.ArtistName,
                    CoverImageUrl = album.CoverImageUrl,
                    ReleaseDate = album.ReleaseDate,
                    Genre = album.Genre,
                    TotalTracks = album.TotalTracks,
                    Duration = album.Duration,
                    CreatedAt = album.CreatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi tạo album.", Error = ex.Message });
            }
        }

        // Lấy danh sách tất cả album
        [HttpGet]
        public async Task<IActionResult> GetAllAlbums()
        {
            try
            {
                var albums = await _context.Albums
                    .Select(a => new AlbumResponse
                    {
                        AlbumId = a.AlbumId,
                        AlbumTitle = a.AlbumTitle,
                        ArtistName = a.ArtistName,
                        CoverImageUrl = a.CoverImageUrl,
                        ReleaseDate = a.ReleaseDate,
                        Genre = a.Genre,
                        TotalTracks = a.TotalTracks,
                        Duration = a.Duration,
                        CreatedAt = a.CreatedAt
                    })
                    .ToListAsync();

                return Ok(albums);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy danh sách album.", Error = ex.Message });
            }
        }

        // Lấy chi tiết album
        [HttpGet("{albumId}")]
        public async Task<IActionResult> GetAlbumDetail(int albumId)
        {
            try
            {
                var album = await _context.Albums
                    .Include(a => a.Sounds)
                    .FirstOrDefaultAsync(a => a.AlbumId == albumId);

                if (album == null)
                    return NotFound(new { Message = "Album không tồn tại." });

                var response = new AlbumDetailResponse
                {
                    AlbumId = album.AlbumId,
                    AlbumTitle = album.AlbumTitle,
                    ArtistName = album.ArtistName,
                    CoverImageUrl = album.CoverImageUrl,
                    ReleaseDate = album.ReleaseDate,
                    Genre = album.Genre,
                    TotalTracks = album.TotalTracks,
                    Duration = album.Duration,
                    CreatedAt = album.CreatedAt,
                    Sounds = album.Sounds.Select(s => new SoundResponse
                    {
                        SoundId = s.SoundId,
                        Title = s.Title,
                        ArtistName = s.ArtistName,
                        AlbumId = s.AlbumId,
                        CategoryId = s.CategoryId,
                        Duration = s.Duration,
                        FileUrl = s.FileUrl,
                        CoverImageUrl = s.CoverImageUrl,
                        Lyrics = s.Lyrics,
                        PlayCount = s.PlayCount,
                        LikeCount = s.LikeCount,
                        IsPublic = s.IsPublic,
                        IsActive = s.IsActive,
                        UploadedBy = s.UploadedBy,
                        CreatedAt = s.CreatedAt
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi lấy chi tiết album.", Error = ex.Message });
            }
        }

        // Cập nhật album
        [HttpPut("{albumId}")]
        public async Task<IActionResult> UpdateAlbum(int albumId, [FromBody] UpdateAlbumRequest request)
        {
            try
            {
                var album = await _context.Albums.FindAsync(albumId);
                if (album == null)
                    return NotFound(new { Message = "Album không tồn tại." });

                // Kiểm tra dữ liệu đầu vào
                if (request.AlbumTitle != null)
                {
                    if (string.IsNullOrWhiteSpace(request.AlbumTitle))
                        return BadRequest(new { Message = "Tên album không được để trống." });
                    if (request.AlbumTitle.Length > 200)
                        return BadRequest(new { Message = "Tên album không được vượt quá 200 ký tự." });
                    album.AlbumTitle = request.AlbumTitle;
                }

                if (request.ArtistName != null)
                {
                    if (string.IsNullOrWhiteSpace(request.ArtistName))
                        return BadRequest(new { Message = "Tên nghệ sĩ không được để trống." });
                    if (request.ArtistName.Length > 100)
                        return BadRequest(new { Message = "Tên nghệ sĩ không được vượt quá 100 ký tự." });
                    album.ArtistName = request.ArtistName;
                }

                if (request.CoverImageUrl != null && request.CoverImageUrl.Length > 500)
                    return BadRequest(new { Message = "URL hình ảnh bìa không được vượt quá 500 ký tự." });

                if (request.Genre != null && request.Genre.Length > 50)
                    return BadRequest(new { Message = "Thể loại không được vượt quá 50 ký tự." });

                // Cập nhật các trường
                album.CoverImageUrl = request.CoverImageUrl ?? album.CoverImageUrl;
                album.ReleaseDate = request.ReleaseDate ?? album.ReleaseDate; // DateOnly?
                album.Genre = request.Genre ?? album.Genre;

                await _context.SaveChangesAsync();

                // Trả về thông tin album đã cập nhật
                var response = new AlbumResponse
                {
                    AlbumId = album.AlbumId,
                    AlbumTitle = album.AlbumTitle,
                    ArtistName = album.ArtistName,
                    CoverImageUrl = album.CoverImageUrl,
                    ReleaseDate = album.ReleaseDate,
                    Genre = album.Genre,
                    TotalTracks = album.TotalTracks,
                    Duration = album.Duration,
                    CreatedAt = album.CreatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi cập nhật album.", Error = ex.Message });
            }
        }

        // Xóa album
        [HttpDelete("{albumId}")]
        public async Task<IActionResult> DeleteAlbum(int albumId)
        {
            try
            {
                var album = await _context.Albums.FindAsync(albumId);
                if (album == null)
                    return NotFound(new { Message = "Album không tồn tại." });

                _context.Albums.Remove(album);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Xóa album thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi xóa album.", Error = ex.Message });
            }
        }
    }
}