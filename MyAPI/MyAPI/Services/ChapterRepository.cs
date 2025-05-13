using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Db;
using MyAPI.Dtos;
using MyAPI.Interface;

namespace MyAPI.Services
{
    public class ChapterRepository : Repository<Chapter>, IChapterRepository
    {
        public ChapterRepository(MyDbContext context) : base(context)
        {
        }

        public async Task<List<ChapterResponseDto>> GetChaptersByStoryIdAsync(string storyId)
        {
            var chapters = await _context.Chapters
                .Where(c => c.StoryId == storyId)
                .OrderBy(c => c.ChapterNumber)
                .ToListAsync();

            return chapters.Select(c => MapToChapterResponseDto(c)).ToList();
        }

        public async Task<ChapterResponseDto> GetChapterWithDetailsAsync(string id)
        {
            var chapter = await _context.Chapters
                .Include(c => c.Story)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chapter == null)
                return null;

            return MapToChapterResponseDto(chapter);
        }

        public async Task<ChapterResponseDto> GetChapterByStoryAndNumberAsync(string storyId, int chapterNumber)
        {
            var chapter = await _context.Chapters
                .FirstOrDefaultAsync(c => c.StoryId == storyId && c.ChapterNumber == chapterNumber);

            if (chapter == null)
                return null;

            return MapToChapterResponseDto(chapter);
        }

        public async Task<int> GetChapterCountByStoryIdAsync(string storyId)
        {
            return await _context.Chapters
                .AsNoTracking()
                .CountAsync(c => c.StoryId == storyId);
        }


        //public async Task UpdateViewsAsync(string chapterId)
        //{
        //    var chapter = await _context.Chapters.FindAsync(chapterId);
        //    if (chapter != null)
        //    {
        //        chapter.Views++;
        //        chapter.UpdatedAt = DateTime.UtcNow;

        //        var story = await _context.Stories.FindAsync(chapter.StoryId);
        //        if (story != null)
        //        {
        //            story.TotalViews++;
        //            story.UpdatedAt = DateTime.UtcNow;
        //        }
        //    }
        //}

        private static string InsertLineBreaksAtPeriods(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            var parts = input
                .Split('.', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .Where(p => p.Length > 0);

            return string.Join(Environment.NewLine, parts.Select(p => p + "."));
        }

        private ChapterResponseDto MapToChapterResponseDto(Chapter chapter)
        {
            return new ChapterResponseDto
            {
                Id = chapter.Id,
                StoryId = chapter.StoryId,
                ChapterNumber = chapter.ChapterNumber,
                Title = chapter.Title,
                Content = InsertLineBreaksAtPeriods(chapter.Content),
                Views = chapter.Views,
                CreatedAt = chapter.CreatedAt,
                UpdatedAt = chapter.UpdatedAt,
                CommentsCount = chapter.CommentsCount
            };
        }

        public Task<string> GetFirstChapterUrl(string storyId)
        {
            return _context.Chapters
                .Where(c => c.StoryId == storyId)
                .OrderBy(c => c.ChapterNumber)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<List<LatestChapterDto>> GetLatestChaptersAsync(int top = 10)
        {
            return await _context.Chapters
                .AsNoTracking()
                .Include(c => c.Story)
                    .ThenInclude(s => s.Categories)
                .OrderByDescending(c => c.CreatedAt)
                .Take(top)
                .Select(c => new LatestChapterDto
                {
                    StoryId = c.StoryId,
                    StoryTitle = c.Story.Title,
                    Author = c.Story.Author,
                    Categories = c.Story.Categories.Select(cat => cat.Name).ToList(),
                    ChapterNumber = c.ChapterNumber,
                    ChapterTitle = c.Title,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();
        }

       

        public async Task<ViewCountDto> UpdateViewsAsync(string chapterId)
        {
            var chapter = await _context.Chapters.FindAsync(chapterId);
            if (chapter == null)
                return null!;

            chapter.Views++;
            chapter.UpdatedAt = DateTime.Now;

            var story = await _context.Stories.FindAsync(chapter.StoryId);
            if (story != null)
            {
                story.TotalViews++;
                story.UpdatedAt = DateTime.Now;
            }

            await _context.SaveChangesAsync();

            return new ViewCountDto
            {
                ChapterViews = chapter.Views,
                StoryViews = story?.TotalViews ?? 0L
            };
        }


    }
}
