namespace MyAPI.Dtos
{
    public class CommentResponseDto
    {
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string StoryId { get; set; }
        public string ChapterId { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }
        public string ParentCommentId { get; set; }
        public bool HasReplies { get; set; }
    }
}
