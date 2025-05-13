using MyAPI.Data;
using MyAPI.Dtos.Rating;

namespace MyAPI.Interface
{
    public interface IRatingRepository : IRepository<Rating>
    {
        Task<List<RatingResponseDto>> GetRatingsByStoryIdAsync(string storyId);
        Task<RatingResponseDto> GetUserRatingForStoryAsync(string storyId, Guid userId);
        // Tạo mới rating
        Task<RatingResponseDto> AddRatingAsync(string storyId, Guid userId, double score, string review);
        // Cập nhật rating đã tồn tại
        

        // Cập nhật summary (average/count) lên Story
        Task UpdateStoryRatingSummaryAsync(string storyId);
        Task<RatingResponseDto> UpdateRatingAsync(string storyId, Guid userId, double score, string review);
    }
}
