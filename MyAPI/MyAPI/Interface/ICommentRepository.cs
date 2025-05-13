using MyAPI.Data;
using MyAPI.Dtos;

namespace MyAPI.Interface
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<List<CommentResponseDto>> GetCommentsByStoryIdAsync(string storyId);
        Task<List<CommentResponseDto>> GetCommentsByChapterIdAsync(string chapterId);
        Task<List<CommentResponseDto>> GetRepliesAsync(string parentCommentId);
        Task UpdateLikesCountAsync(string commentId, int count);
        Task<CommentResponseDto> CreateAsync(CreateCommentDto dto, Guid userId);
    }
}
