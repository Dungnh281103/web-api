using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyAPI.Data
{
    public class Rating
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

        [Range(0, 5)]
        public double Score { get; set; }

        public string Review { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int LikesCount { get; set; }
    }
}
