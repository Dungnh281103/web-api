namespace MyAPI.Dtos
{
   
    public class ReadingHistoryDto
    {
        public string StoryId { get; set; }
        public string StoryTitle { get; set; }
        public string LastChapterId { get; set; }
        public string CoverUrl { get; set; }
        public int LastChapterNumber { get; set; }
        public DateTime LastReadAt { get; set; }
    }

}
