using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Db;
using MyAPI.Dtos;
using MyAPI.Interface;

namespace MyAPI.Services
{

    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(MyDbContext context) : base(context)
        {
        }

        public async Task<List<CategoryDto>> GetAllCategoriesWithCountAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Stories)
                .ToListAsync();

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
            }).ToList();
        }

        public async Task<CategoryDto> GetCategoryWithDetailsAsync(string id)
        {
            var category = await _context.Categories
                .Include(c => c.Stories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
            };
        }

        public async Task UpdateStoryCountAsync(string categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Stories)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category != null)
            {
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Category> GetByIdAsync(string id)
        {
            return await _context.Categories
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category> GetByNameAsync(string name)
        {
            return await _context.Categories
                                 .FirstOrDefaultAsync(c => c.Name == name);
        }
    }
}
