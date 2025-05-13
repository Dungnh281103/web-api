using MyAPI.Dtos;
using MyAPI.Dtos.SavedStory;

namespace MyAPI.Interface
{
    public interface ISavedStoryRepository
    {
        Task<SavedStoryResponseDto> SaveAsync(Guid userId, string storyId);
        Task<bool> UnsaveAsync(Guid userId, string storyId);
        Task<List<SavedStoryResponseDto>> GetSavedByUserAsync(Guid userId);
        Task<bool> IsSavedAsync(Guid userId, string storyId);
    }
}
