using MyAPI.Data;

namespace MyAPI.Interface
{
    public interface IReadingHistoryRepository
    {
        Task UpsertAsync(ReadingHistory history);
        Task<List<ReadingHistory>> GetByUserAsync(Guid userId);
        Task<ReadingHistory> GetByUserAndStoryAsync(Guid userId, string storyId);
    }
}
