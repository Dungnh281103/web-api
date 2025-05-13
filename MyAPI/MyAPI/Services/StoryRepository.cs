using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Db;
using MyAPI.Dtos;
using MyAPI.Interface;

namespace MyAPI.Services
{
    public class StoryRepository : Repository<Story>, IStoryRepository
    {
        public StoryRepository(MyDbContext context) : base(context)
        {
        }

        public async Task<List<StoryResponseDto>> GetAllStoriesWithDetailsAsync(string title = null)
        {
            var query = _context.Stories
                .Include(s => s.Categories)
                .Include(s => s.Tags)
                .AsQueryable();

            // Filter by title if provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                query = query.Where(s => s.Title.Contains(title));
            }

            var stories = await query.ToListAsync();
            return stories.Select(s => MapToStoryResponseDto(s)).ToList();
        }

        public async Task<StoryResponseDto> GetStoryWithDetailsAsync(string id)
        {
            var story = await _context.Stories
                .Include(s => s.Categories)
                .Include(s => s.Tags)
                .Include(s => s.Ratings)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (story == null)
                return null;

            return MapToStoryResponseDto(story);
        }

        public async Task<List<StoryResponseDto>> GetRecommendedStoriesAsync()
        {
            var stories = await _context.Stories
                .Where(s => s.IsRecommended)
                .Include(s => s.Categories)
                .Include(s => s.Tags)
                .ToListAsync();

            return stories.Select(s => MapToStoryResponseDto(s)).ToList();
        }

        public async Task<List<HotStoryDto>> GetHotStoriesAsync(int top)
        {
            return await _context.Stories
                .OrderByDescending(s => s.TotalViews)
                .Take(top)
                .Select(s => new HotStoryDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Author = s.Author,
                    TotalViews = s.TotalViews,
                    Status = s.Status,
                    CoverUrl = s.CoverUrl
                })
                .ToListAsync();
        }

        public async Task<List<StoryResponseDto>> GetStoriesByCategoryAsync(string categoryId)
        {
            var stories = await _context.Stories
                .Include(s => s.Categories)
                .Include(s => s.Tags)
                .Where(s => s.Categories.Any(c => c.Id == categoryId))
                .ToListAsync();

            return stories.Select(s => MapToStoryResponseDto(s)).ToList();
        }

        public async Task<List<StoryResponseDto>> GetStoriesByTagAsync(string tagId)
        {
            var stories = await _context.Stories
                .Include(s => s.Categories)
                .Include(s => s.Tags)
                .Where(s => s.Tags.Any(t => t.Id == tagId))
                .ToListAsync();

            return stories.Select(s => MapToStoryResponseDto(s)).ToList();
        }

        public async Task UpdateViewsAsync(string storyId)
        {
            var story = await _context.Stories.FindAsync(storyId);
            if (story != null)
            {
                story.TotalViews++;
                story.UpdatedAt = DateTime.Now;
            }
        }

        public async Task UpdateBookmarksCountAsync(string storyId, int count)
        {
            var story = await _context.Stories.FindAsync(storyId);
            if (story != null)
            {
                story.BookmarksCount = count;
                story.UpdatedAt = DateTime.Now;
            }
        }

        public async Task<List<CompletedStoryDto>> GetCompletedStoriesAsync()
        {
            return await _context.Stories
                .Where(s => s.Status == "Hoàn thành")   
                .Select(s => new CompletedStoryDto
                {
                    Id = s.Id,
                    Title = s.Title,
                    Author = s.Author,
                    Status = s.Status,
                    TotalViews = s.TotalViews,
                    CoverUrl = s.CoverUrl         
                })
                .ToListAsync();
        }

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


        private StoryResponseDto MapToStoryResponseDto(Story story)
        {
            return new StoryResponseDto
            {
                Id = story.Id,
                Title = story.Title,
                Author = story.Author,
                Translator = story.Translator,
                CoverUrl = story.CoverUrl,
                Description = InsertLineBreaksAtPeriods(story.Description),
                Categories = story.Categories?.Select(c => c.Name).ToList() ?? new List<string>(),
                Tags = story.Tags?.Select(t => t.Name).ToList() ?? new List<string>(),
                Status = story.Status,
                IsRecommended = story.IsRecommended,
                TotalChapters = story.TotalChapters,
                TotalViews = story.TotalViews,
                Rating = new RatingDto
                {
                    Average = story.Rating?.Average ?? 0,
                    Count = story.Rating?.Count ?? 0
                },
                BookmarksCount = story.BookmarksCount,
                CreatedAt = story.CreatedAt,
                UpdatedAt = story.UpdatedAt,
                LastChapterUpdate = story.LastChapterUpdate
            };
        }

       
    }
}
