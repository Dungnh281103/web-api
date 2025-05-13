using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Db;
using MyAPI.Dtos;
using MyAPI.Dtos.SavedStory;
using MyAPI.Interface;

namespace MyAPI.Services
{
    public class SavedStoryRepository : ISavedStoryRepository
    {
        private readonly MyDbContext _context;

        public SavedStoryRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<SavedStoryResponseDto> SaveAsync(Guid userId, string storyId)
        {
            if (await _context.SavedStories.AnyAsync(x => x.UserId == userId && x.StoryId == storyId))
                return null; // đã lưu rồi

            var saved = new SavedStory
            {
                UserId = userId,
                StoryId = storyId,
                SavedAt = DateTime.Now
            };
            _context.SavedStories.Add(saved);
            await _context.SaveChangesAsync();

            return new SavedStoryResponseDto
            {
                Id = saved.Id,
                StoryId = saved.StoryId,
                SavedAt = saved.SavedAt
            };
        }

        public async Task<bool> UnsaveAsync(Guid userId, string storyId)
        {
            var saved = await _context.SavedStories
                .FirstOrDefaultAsync(x => x.UserId == userId && x.StoryId == storyId);
            if (saved == null) return false;

            _context.SavedStories.Remove(saved);
            await _context.SaveChangesAsync();
            return true;
        }
        //guid : định dạng muốn truyền vào(chuối kí tự ngẫu nhiên), truyền userid 
        //List<SavedStoryResponseDto> trả về nhiều cái savesstoryReponsedto
        public async Task<List<SavedStoryResponseDto>> GetSavedByUserAsync(Guid userId) 
        {
            var list = await _context.SavedStories
                .Where(x => x.UserId == userId)
                .Include(x => x.Story)
                .OrderByDescending(x => x.SavedAt) // sap xep giam dan
                .ToListAsync();

            return list.Select(x => new SavedStoryResponseDto
            {
                Id = x.Id,
                StoryId = x.StoryId,
                Title = x.Story.Title,                  // <-- map Title
                Author = x.Story.Author,
                SavedAt = x.SavedAt
            }).ToList();
        }

        public async Task<bool> IsSavedAsync(Guid userId, string storyId)
        {
            return await _context.SavedStories
                .AnyAsync(x => x.UserId == userId && x.StoryId == storyId);
        }
    }

}
