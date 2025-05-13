namespace MyAPI.Dtos
{
    public class CreateCommentDto
    {
        public string StoryId { get; set; }
        public string ChapterId { get; set; }
        public string Content { get; set; }
        public string? ParentCommentId { get; set; }
    }
}
