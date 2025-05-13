namespace MyAPI.Dtos
{
    public class ReportResponseDto
    {
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string StoryId { get; set; }
        public string StoryTitle { get; set; }
        public string ChapterId { get; set; }
        public string ChapterTitle { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
