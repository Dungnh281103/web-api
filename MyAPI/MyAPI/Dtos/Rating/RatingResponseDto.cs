using System.ComponentModel.DataAnnotations;

namespace MyAPI.Dtos.Rating
{
    public class RatingResponseDto
    {
        public string Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string StoryId { get; set; }
        public double Score { get; set; }
        public string Review { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int LikesCount { get; set; }
    }
   
    

}
