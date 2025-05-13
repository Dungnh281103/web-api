using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Db;
using MyAPI.Dtos;
using MyAPI.Interface;

namespace MyAPI.Services
{
    public class ReportRepository : Repository<Report>, IReportRepository
    {
        public ReportRepository(MyDbContext context) : base(context)
        {
        }

        public async Task<List<ReportResponseDto>> GetReportsByStoryIdAsync(string storyId)
        {
            var reports = await _context.Reports
                .Include(r => r.User)
                .Include(r => r.Story)
                .Where(r => r.StoryId == storyId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reports.Select(r => MapToReportResponseDto(r)).ToList();
        }

        public async Task<List<ReportResponseDto>> GetReportsByChapterIdAsync(string chapterId)
        {
            var reports = await _context.Reports
                .Include(r => r.User)
                .Include(r => r.Chapter)
                .Where(r => r.ChapterId == chapterId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reports.Select(r => MapToReportResponseDto(r)).ToList();
        }

        public async Task<List<ReportResponseDto>> GetReportsByStatusAsync(string status)
        {
            var reports = await _context.Reports
                .Include(r => r.User)
                .Include(r => r.Story)
                .Include(r => r.Chapter)
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return reports.Select(r => MapToReportResponseDto(r)).ToList();
        }

        private ReportResponseDto MapToReportResponseDto(Report report)
        {
            return new ReportResponseDto
            {
                Id = report.Id,
                UserId = report.UserId,
                UserName = report.User?.UserName,
                StoryId = report.StoryId,
                StoryTitle = report.Story?.Title,
                ChapterId = report.ChapterId,
                ChapterTitle = report.Chapter?.Title,
                Type = report.Type,
                Description = report.Description,
                Status = report.Status,
                CreatedAt = report.CreatedAt,
                UpdatedAt = report.UpdatedAt
            };
        }
    }

}
