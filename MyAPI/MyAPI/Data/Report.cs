using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyAPI.Data
{
    public class Report
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

        public string Type { get; set; }
        public string Description { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
