using System.Text.Json.Serialization;

namespace ReadingBookAPI.Models
{
    public class BookItem
    {
        [JsonIgnore]
        public long Id { get; set; }
        [JsonIgnore]
        public int rank { get; set; }
        public string? UserName { get; set; } = string.Empty;
        public int pageNumber { get; set; }
        public int TodayPageNumber { get; set; }
        [JsonIgnore]
        public int StreakNumber { get; set; }
        [JsonIgnore]
        public int TotalPages { get; set; } = 0;


        [JsonIgnore]
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
    }
}
