using System.ComponentModel.DataAnnotations;

namespace MyAPI.Dtos.Rating
{
    public class RatingUpdateDto
    {
        [Required]
        [Range(0, 5)]
        public double Score { get; set; }

        public string Review { get; set; }
    }
}
