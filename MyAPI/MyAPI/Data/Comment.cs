using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace MyAPI.Data
{
    public class Comment
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }

        [Required]
        public string StoryId { get; set; }
        [ForeignKey(nameof(StoryId))]
        public Story Story { get; set; }

        public string ChapterId { get; set; }
        [ForeignKey(nameof(ChapterId))]
        public Chapter Chapter { get; set; }

        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public int LikesCount { get; set; }

        // Self-referencing relationship for replies
        public string? ParentCommentId { get; set; }
        [ForeignKey(nameof(ParentCommentId))]
        public Comment? ParentComment { get; set; }

        public ICollection<Comment> Replies { get; set; }
    }
}