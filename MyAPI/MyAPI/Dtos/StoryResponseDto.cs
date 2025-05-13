namespace MyAPI.Dtos
{
    public class StoryResponseDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Translator { get; set; }
        public string CoverUrl { get; set; }
        public string Description { get; set; }
        public List<string> Categories { get; set; }
        public List<string> Tags { get; set; }
        public string Status { get; set; }
        public bool IsRecommended { get; set; }
        public int TotalChapters { get; set; }
        public long TotalViews { get; set; }
        public RatingDto Rating { get; set; }
        public int BookmarksCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime LastChapterUpdate { get; set; }
    }

    public class RatingDto
    {
        public double Average { get; set; }
        public int Count { get; set; }
    }
}
