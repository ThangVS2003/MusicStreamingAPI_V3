using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicStreamingAPI.Models;
using MusicStreamingAPI.DTOs;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStreamingAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SoundsController : ControllerBase
    {
        private readonly MusicStreamingDbContext _context;
        private readonly IConfiguration _configuration;

        public SoundsController(MusicStreamingDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> UploadSound([FromForm] UploadSoundRequest request)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (string.IsNullOrWhiteSpace(request.Title))
                    return BadRequest(new { Message = "Tên bài nhạc không được để trống." });
                if (request.Title.Length > 200)
                    return BadRequest(new { Message = "Tên bài nhạc không được vượt quá 200 ký tự." });
                if (string.IsNullOrWhiteSpace(request.ArtistName))
                    return BadRequest(new { Message = "Tên nghệ sĩ không được để trống." });
                if (request.ArtistName.Length > 100)
                    return BadRequest(new { Message = "Tên nghệ sĩ không được vượt quá 100 ký tự." });
                if (request.File == null || request.File.Length == 0)
                    return BadRequest(new { Message = "File âm thanh không được để trống." });
                if (request.Duration <= 0)
                    return BadRequest(new { Message = "Thời lượng bài nhạc phải lớn hơn 0." });
                if (request.CoverImageUrl != null && request.CoverImageUrl.Length > 500)
                    return BadRequest(new { Message = "URL hình ảnh bìa không được vượt quá 500 ký tự." });
                if (request.Lyrics != null && request.Lyrics.Length > 10000)
                    return BadRequest(new { Message = "Lời bài hát không được vượt quá 10000 ký tự." });

                // Kiểm tra UserId
                var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == request.UploadedBy && u.IsActive == true);
                if (user == null)
                    return BadRequest(new { Message = "Người dùng không tồn tại hoặc không hoạt động." });

                // Kiểm tra AlbumId (nếu có)
                Album? album = null;
                if (request.AlbumId.HasValue)
                {
                    album = await _context.Albums.FindAsync(request.AlbumId.Value);
                    if (album == null)
                        return BadRequest(new { Message = "Album không tồn tại." });
                }

                // Kiểm tra CategoryId (nếu có)
                if (request.CategoryId.HasValue)
                {
                    var category = await _context.Categories.FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId.Value && c.IsActive == true);
                    if (category == null)
                        return BadRequest(new { Message = "Danh mục không tồn tại hoặc không hoạt động." });
                }

                // Khởi tạo Google Drive API với Service Account và Domain-Wide Delegation
                var serviceAccountKeyPath = _configuration["Google:ServiceAccountKeyPath"];
                var folderId = _configuration["Google:DriveFolderId"];
                var userEmail = _configuration["Google:UserEmail"];
                var credential = GoogleCredential.FromFile(serviceAccountKeyPath)
                    .CreateScoped(DriveService.Scope.DriveFile)
                    .CreateWithUser(userEmail); // Giả lập tài khoản cá nhân

                var driveService = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "MusicStreamingApp"
                });

                // Upload file lên Google Drive
                string fileId;
                using (var stream = request.File.OpenReadStream())
                {
                    var fileMetadata = new Google.Apis.Drive.v3.Data.File
                    {
                        Name = $"{Guid.NewGuid()}_{request.File.FileName}",
                        MimeType = request.File.ContentType,
                        Parents = new List<string> { folderId }
                    };

                    var uploadRequest = driveService.Files.Create(fileMetadata, stream, request.File.ContentType);
                    uploadRequest.Fields = "id";
                    var uploadProgress = await uploadRequest.UploadAsync();

                    if (uploadProgress.Status != UploadStatus.Completed)
                        return StatusCode(500, new { Message = "Lỗi khi upload file lên Google Drive.", Error = uploadProgress.Exception?.Message });

                    fileId = uploadRequest.ResponseBody.Id;
                }

                // Set quyền chia sẻ công khai
                var permission = new Google.Apis.Drive.v3.Data.Permission
                {
                    Type = "anyone",
                    Role = "reader"
                };
                await driveService.Permissions.Create(permission, fileId).ExecuteAsync();

                // Tạo URL tải xuống
                var fileUrl = $"https://drive.google.com/uc?export=download&id={fileId}";

                // Tạo bài nhạc mới
                var sound = new Sound
                {
                    Title = request.Title,
                    ArtistName = request.ArtistName,
                    AlbumId = request.AlbumId,
                    CategoryId = request.CategoryId,
                    Duration = request.Duration,
                    FileUrl = fileUrl,
                    CoverImageUrl = request.CoverImageUrl,
                    Lyrics = request.Lyrics,
                    PlayCount = 0,
                    LikeCount = 0,
                    IsPublic = request.IsPublic ?? true,
                    IsActive = true,
                    UploadedBy = request.UploadedBy,
                    CreatedAt = DateTime.Now
                };

                _context.Sounds.Add(sound);
                await _context.SaveChangesAsync();

                // Cập nhật TotalTracks và Duration của album (nếu có)
                if (album != null)
                {
                    album.TotalTracks = await _context.Sounds.CountAsync(s => s.AlbumId == album.AlbumId);
                    album.Duration = await _context.Sounds.Where(s => s.AlbumId == album.AlbumId).SumAsync(s => s.Duration);
                    await _context.SaveChangesAsync();
                }

                // Trả về thông tin bài nhạc
                var response = new SoundResponse
                {
                    SoundId = sound.SoundId,
                    Title = sound.Title,
                    ArtistName = sound.ArtistName,
                    AlbumId = sound.AlbumId,
                    CategoryId = sound.CategoryId,
                    Duration = sound.Duration,
                    FileUrl = sound.FileUrl,
                    CoverImageUrl = sound.CoverImageUrl,
                    Lyrics = sound.Lyrics,
                    PlayCount = sound.PlayCount,
                    LikeCount = sound.LikeCount,
                    IsPublic = sound.IsPublic,
                    IsActive = sound.IsActive,
                    UploadedBy = request.UploadedBy,
                    CreatedAt = sound.CreatedAt
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi khi upload bài nhạc.", Error = ex.Message });
            }
        }


    }
}