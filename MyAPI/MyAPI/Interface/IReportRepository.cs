using MyAPI.Data;
using MyAPI.Dtos;

namespace MyAPI.Interface
{
    public interface IReportRepository : IRepository<Report>
    {
        Task<List<ReportResponseDto>> GetReportsByStoryIdAsync(string storyId);
        Task<List<ReportResponseDto>> GetReportsByChapterIdAsync(string chapterId);
        Task<List<ReportResponseDto>> GetReportsByStatusAsync(string status);
    }
}
