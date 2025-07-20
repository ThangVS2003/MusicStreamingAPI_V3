using MusicStreamingAPI.Models;

namespace MusicStreamingAPI.DTOs
{
    // DTOs
    public class SearchResultsDto
    {
        public List<SoundDto>? Sounds { get; set; }
        public List<AlbumDto>? Albums { get; set; }
        public List<ArtistDto>? Artists { get; set; }
        public List<CategoryDto>? Categories { get; set; }
    }

   
   
  
}
