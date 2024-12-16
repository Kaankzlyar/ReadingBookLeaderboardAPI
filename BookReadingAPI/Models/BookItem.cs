using System.Text.Json.Serialization;

namespace ReadingBookAPI.Models
{
    public class BookItem
    {
        
        public long Id { get; set; }

        [JsonIgnore]
        public int Rank { get; set; }
        public string? Username { get; set; } = string.Empty;
        public int TotalPages { get; set; }
        public int TodayPages { get; set; }

        [JsonIgnore]
        public int Streak { get; set; }

        [JsonIgnore]
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
    }
}
