using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyAPI.Data
{
    public class Chapter
    {
        [Key]
        [Required]
        public string Id { get; set; }

        [Required]
        public string StoryId { get; set; }

        [ForeignKey(nameof(StoryId))]
        public Story Story { get; set; }

        public int ChapterNumber { get; set; }

        [StringLength(200)]
        public string Title { get; set; }

        public string Content { get; set; }

        public long Views { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int CommentsCount { get; set; }
    }
}
