namespace MyAPI.Dtos
{
    public class ChapterResponseDto
    {
        public string Id { get; set; }
        public string StoryId { get; set; }
        public int ChapterNumber { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public long Views { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CommentsCount { get; set; }
    }
}
