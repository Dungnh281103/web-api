using MyAPI.Data;
using MyAPI.Dtos;

namespace MyAPI.Interface
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<List<CategoryDto>> GetAllCategoriesWithCountAsync();
        Task<CategoryDto> GetCategoryWithDetailsAsync(string id);
        Task UpdateStoryCountAsync(string categoryId);
        Task<Category> GetByIdAsync(string id);
        Task<Category> GetByNameAsync(string name);
    }
}
