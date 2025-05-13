using MyAPI.Data;
using MyAPI.Dtos;

namespace MyAPI.Interface
{
    public interface IChapterRepository : IRepository<Chapter>
    {
        Task<string> GetFirstChapterUrl(string storyId);
        Task<List<ChapterResponseDto>> GetChaptersByStoryIdAsync(string storyId);
        Task<ChapterResponseDto> GetChapterWithDetailsAsync(string id);
        Task<ChapterResponseDto> GetChapterByStoryAndNumberAsync(string storyId, int chapterNumber);
        Task<List<LatestChapterDto>> GetLatestChaptersAsync(int top = 10);
        Task<int> GetChapterCountByStoryIdAsync(string storyId);
        Task<ViewCountDto> UpdateViewsAsync(string chapterId);
    }
}
