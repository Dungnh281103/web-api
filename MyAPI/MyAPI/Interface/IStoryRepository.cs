using MyAPI.Data;
using MyAPI.Dtos;

namespace MyAPI.Interface
{
    public interface IStoryRepository : IRepository<Story>
    {
        Task<List<StoryResponseDto>> GetAllStoriesWithDetailsAsync(string title = null); //StoryResponseDto là dữ liệu mà bạn muốn trả về
        Task<StoryResponseDto> GetStoryWithDetailsAsync(string id);
        Task<List<StoryResponseDto>> GetRecommendedStoriesAsync();
        Task<List<StoryResponseDto>> GetStoriesByCategoryAsync(string categoryId);
        Task<List<StoryResponseDto>> GetStoriesByTagAsync(string tagId);
        Task<List<HotStoryDto>> GetHotStoriesAsync(int top);
        Task<List<CompletedStoryDto>> GetCompletedStoriesAsync();
        Task UpdateViewsAsync(string storyId);
        Task UpdateBookmarksCountAsync(string storyId, int count);
    }
}

