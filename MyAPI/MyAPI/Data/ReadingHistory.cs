using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Data
{
    public class ReadingHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }
        public string CoverUrl { get; set; }

        [Required]
        public string StoryId { get; set; }

        // ← thêm property này
        [ForeignKey(nameof(StoryId))]
        public Story Story { get; set; }

        [Required]
        public string ChapterId { get; set; }

        // ← nếu bạn muốn truy vấn Chapter luôn
        [ForeignKey(nameof(ChapterId))]
        public Chapter Chapter { get; set; }

        [Required]
        public int ChapterNumber { get; set; }

        [Required]
        public DateTime LastReadAt { get; set; }
    }
}
