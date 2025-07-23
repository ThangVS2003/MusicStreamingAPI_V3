// MusicStreamingAPI/DTOs/DeleteHistoryRequest.cs
using System.Collections.Generic;

namespace MusicStreamingAPI.DTOs
{
    public class DeleteHistoryRequest
    {
        public List<int> HistoryIds { get; set; }
    }
}