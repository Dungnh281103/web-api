namespace MyAPI.Dtos
{
    public class LatestChapterDto
    {
        public string StoryId { get; set; }
        public string StoryTitle { get; set; }
        public string Author { get; set; }
        public List<string> Categories { get; set; }
        public int ChapterNumber { get; set; }
        public string ChapterTitle { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
