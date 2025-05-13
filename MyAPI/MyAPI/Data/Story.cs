using Azure;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyAPI.Data
{
    public class Story
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [StringLength(100)]
        public string Author { get; set; }

        [StringLength(100)]
        public string Translator { get; set; }

        public string CoverUrl { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public bool IsRecommended { get; set; }

        public int TotalChapters { get; set; }

        public long TotalViews { get; set; }

        public int BookmarksCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DateTime LastChapterUpdate { get; set; }

        
        public RatingSummary Rating { get; set; }
        [JsonIgnore]
        public  ICollection<Chapter> Chapters { get; set; }
        [JsonIgnore]
        public ICollection<Comment> Comments { get; set; }
        [JsonIgnore]
        public ICollection<Rating> Ratings { get; set; }

        // Nhiều-nhiều với Category & Tag (EF Core 5+ sẽ tự tạo bảng liên kết)
        [JsonIgnore]
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        [JsonIgnore]
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();
        [JsonIgnore]
        public ICollection<ReadingHistory> ReadingHistories { get; set; } = new List<ReadingHistory>();
    
}

    [Owned]
    public class RatingSummary
    {
        public double Average { get; set; }
        public int Count { get; set; }
    }
}
