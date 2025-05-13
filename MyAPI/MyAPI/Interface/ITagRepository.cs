using MyAPI.Data;
using MyAPI.Dtos;

namespace MyAPI.Interface
{
    public interface ITagRepository : IRepository<Tag>
    {
        Task<List<TagDto>> GetAllTagsWithCountAsync();
        Task<TagDto> GetTagWithDetailsAsync(string id);
        Task<Tag> GetByIdAsync(string id);
        Task<Tag> GetByNameAsync(string name);
        Task UpdateStoryCountAsync(string tagId);
    }
}
