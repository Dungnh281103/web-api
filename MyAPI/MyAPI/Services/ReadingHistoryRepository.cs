using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Db;
using MyAPI.Interface;

namespace MyAPI.Services
{
    public class ReadingHistoryRepository : IReadingHistoryRepository
    {
        private readonly MyDbContext _context;

        public ReadingHistoryRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task UpsertAsync(ReadingHistory history)
        {

            var storyCover = await _context.Stories
                .Where(s => s.Id == history.StoryId)
                .Select(s => s.CoverUrl)
                .FirstOrDefaultAsync();

            var exist = await _context.ReadingHistories
                .FirstOrDefaultAsync(r =>
                    r.UserId == history.UserId &&
                    r.StoryId == history.StoryId);

            if (exist == null)
            {
                history.LastReadAt = DateTime.Now;
                history.CoverUrl = storyCover;
                await _context.ReadingHistories.AddAsync(history);
            }
            else
            {
                exist.ChapterId = history.ChapterId;
                exist.ChapterNumber = history.ChapterNumber;
                exist.LastReadAt = DateTime.Now;
                exist.CoverUrl = storyCover;
                _context.ReadingHistories.Update(exist);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<ReadingHistory>> GetByUserAsync(Guid userId)
        {
            return await _context.ReadingHistories
                .Where(r => r.UserId == userId)
                .Include(r => r.Story)
                .OrderByDescending(r => r.LastReadAt)
                .ToListAsync();
        }

        public async Task<ReadingHistory> GetByUserAndStoryAsync(Guid userId, string storyId)
        {
            return await _context.ReadingHistories
                .Where(r => r.UserId == userId && r.StoryId == storyId)
                .Include(r => r.Story)
                .OrderByDescending(r => r.LastReadAt)
                .FirstOrDefaultAsync();
        }
    }
}
