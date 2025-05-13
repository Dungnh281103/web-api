using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Db;
using MyAPI.Dtos.Rating;
using MyAPI.Interface;

namespace MyAPI.Services
{
    public class RatingRepository : Repository<Rating>, IRatingRepository
    {
        public RatingRepository(MyDbContext context) : base(context)
        {
        }

        public async Task<List<RatingResponseDto>> GetRatingsByStoryIdAsync(string storyId)
        {
            var ratings = await _context.Ratings
                .Include(r => r.User)
                .Where(r => r.StoryId == storyId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return ratings.Select(r => MapToRatingResponseDto(r)).ToList();
        }

        public async Task<RatingResponseDto> GetUserRatingForStoryAsync(string storyId, Guid userId)
        {
            var rating = await _context.Ratings
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.StoryId == storyId && r.UserId == userId);

            if (rating == null)
                return null;

            return MapToRatingResponseDto(rating);
        }

        public async Task<RatingResponseDto> AddRatingAsync(string storyId, Guid userId, double score, string review)
        {
            var r = new Rating
            {
                Id = Guid.NewGuid().ToString(),
                StoryId = storyId,
                UserId = userId,
                Score = score,
                Review = review,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _context.Ratings.AddAsync(r);
            await _context.SaveChangesAsync();
            await UpdateStoryRatingSummaryAsync(storyId);
            await _context.Entry(r).Reference(x => x.User).LoadAsync();
            return MapToRatingResponseDto(r);
        }

        

        public async Task UpdateStoryRatingSummaryAsync(string storyId)
        {
            var story = await _context.Stories.FindAsync(storyId);
            var ratings = await _context.Ratings.Where(r => r.StoryId == storyId).ToListAsync();

            if (story != null)
            {
                story.Rating = new RatingSummary
                {
                    Average = ratings.Any() ? ratings.Average(r => r.Score) : 0,
                    Count = ratings.Count
                };
                story.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }

        private RatingResponseDto MapToRatingResponseDto(Rating rating)
        {
            return new RatingResponseDto
            {
                Id = rating.Id,
                UserId = rating.UserId,
                UserName = rating.User?.UserName,
                StoryId = rating.StoryId,
                Score = rating.Score,
                Review = rating.Review,
                CreatedAt = rating.CreatedAt,
                UpdatedAt = rating.UpdatedAt,
                LikesCount = rating.LikesCount
            };
        }

        public async Task<RatingResponseDto> UpdateRatingAsync(string storyId, Guid userId, double score, string review)
        {
            var rating = await _context.Ratings
                .FirstOrDefaultAsync(r => r.StoryId == storyId && r.UserId == userId);
            if (rating == null) return null;

            rating.Score = score;
            rating.Review = review;
            rating.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            return new RatingResponseDto
            {
                StoryId = rating.StoryId,
                UserId = rating.UserId,
                Score = rating.Score,
                Review = rating.Review,
                UpdatedAt = rating.UpdatedAt
            };
        }


    }
}
