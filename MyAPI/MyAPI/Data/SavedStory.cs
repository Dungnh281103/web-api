using System;
using System.ComponentModel.DataAnnotations;

namespace MyAPI.Data
{
    public class SavedStory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public AppUser User { get; set; }

        [Required]
        public string StoryId { get; set; }
        public Story Story { get; set; }

        public DateTime SavedAt { get; set; } = DateTime.Now;
    }
}
