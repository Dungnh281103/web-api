namespace MyAPI.Dtos
{
    public class HotStoryDto
    {
        public string Id { get; set; }           // StoryID
        public string Title { get; set; }
        public string Author { get; set; }
        public long TotalViews { get; set; }
        public string Status { get; set; }
        public string CoverUrl { get; set; }
    }
}
