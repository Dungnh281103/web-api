using System.ComponentModel.DataAnnotations;

namespace MyAPI.Dtos.Rating
{
    public class RatingCreateDto
    {
        [Required]
        public string StoryId { get; set; }

        [Required]
        [Range(0, 5)]
        public double Score { get; set; }

        public string Review { get; set; }
    }
}
