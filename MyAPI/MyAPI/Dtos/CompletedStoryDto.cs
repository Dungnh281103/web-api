namespace MyAPI.Dtos
{
    public class CompletedStoryDto
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Status { get; set; }
        public long TotalViews { get; set; }
        public string CoverUrl { get; set; } 
    }
}
