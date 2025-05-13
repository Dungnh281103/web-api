using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Db;
using MyAPI.Dtos;
using MyAPI.Interface;

namespace MyAPI.Services
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(MyDbContext context) : base(context)
        {
        }

        public async Task<List<TagDto>> GetAllTagsWithCountAsync()
        {
            var tags = await _context.Tags
                .Include(t => t.Stories)
                .ToListAsync();

            return tags.Select(t => new TagDto
            {
                Id = t.Id,
                Name = t.Name,
                StoryCount = t.StoryCount
            }).ToList();
        }

        public async Task<TagDto> GetTagWithDetailsAsync(string id)
        {
            var tag = await _context.Tags
                .Include(t => t.Stories)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (tag == null)
                return null;

            return new TagDto
            {
                Id = tag.Id,
                Name = tag.Name,
                StoryCount = tag.StoryCount
            };
        }

        public async Task UpdateStoryCountAsync(string tagId)
        {
            var tag = await _context.Tags
                .Include(t => t.Stories)
                .FirstOrDefaultAsync(t => t.Id == tagId);

            if (tag != null)
            {
                tag.StoryCount = tag.Stories?.Count ?? 0;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Tag> GetByIdAsync(string id)
        {
            return await _context.Tags
                                 .FirstOrDefaultAsync(t => t.Id == id);
        }

        // Triển khai GetByNameAsync
        public async Task<Tag> GetByNameAsync(string name)
        {
            return await _context.Tags
                                 .FirstOrDefaultAsync(t => t.Name == name);
        }
    }
}
