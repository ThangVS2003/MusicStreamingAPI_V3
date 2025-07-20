using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.DTOs
{
    // DTOs
    public class CategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public CategoryDto(Category c)
        {
            CategoryId = c.CategoryId;
            CategoryName = c.CategoryName;
            Description = c.Description;
            ImageUrl = c.ImageUrl;
        }
    }
    
}
