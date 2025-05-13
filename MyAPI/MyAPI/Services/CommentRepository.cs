using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Db;
using MyAPI.Dtos;
using MyAPI.Interface;

namespace MyAPI.Services
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        public CommentRepository(MyDbContext context) : base(context)
        {
        }

        public async Task<List<CommentResponseDto>> GetCommentsByStoryIdAsync(string storyId)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.StoryId == storyId && c.ParentCommentId == null)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return comments.Select(c => MapToCommentResponseDto(c)).ToList();
        }

        public async Task<List<CommentResponseDto>> GetCommentsByChapterIdAsync(string chapterId)
        {
            var comments = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.ChapterId == chapterId && c.ParentCommentId == null)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return comments.Select(c => MapToCommentResponseDto(c)).ToList();
        }

        public async Task<List<CommentResponseDto>> GetRepliesAsync(string parentCommentId)
        {
            var replies = await _context.Comments
                .Include(c => c.User)
                .Where(c => c.ParentCommentId == parentCommentId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return replies.Select(c => MapToCommentResponseDto(c)).ToList();
        }

        public async Task UpdateLikesCountAsync(string commentId, int count)
        {
            var comment = await _context.Comments.FindAsync(commentId);
            if (comment != null)
            {
                comment.LikesCount = count;
            }
        }

        private CommentResponseDto MapToCommentResponseDto(Comment comment)
        {
            return new CommentResponseDto
            {
                Id = comment.Id,
                UserId = comment.UserId,
                UserName = comment.User?.UserName,
                StoryId = comment.StoryId,
                ChapterId = comment.ChapterId,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                LikesCount = comment.LikesCount,
                ParentCommentId = comment.ParentCommentId,
                HasReplies = _context.Comments.Any(c => c.ParentCommentId == comment.Id)
            };
        }

        public async Task<CommentResponseDto> CreateAsync(CreateCommentDto dto, Guid userId)
        {
            // 1. Khởi tạo entity Comment
            var comment = new Comment
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userId,
                StoryId = dto.StoryId,
                ChapterId = dto.ChapterId,
                Content = dto.Content,
                CreatedAt = DateTime.Now,
                LikesCount = 0,
                ParentCommentId = string.IsNullOrWhiteSpace(dto.ParentCommentId)
                    ? null
                    : dto.ParentCommentId
            };

            // 2. Thêm vào Context và lưu
            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            // 3. Tải lại comment cùng thông tin User để mapping
            var created = await _context.Comments
                .Include(c => c.User)
                .FirstAsync(c => c.Id == comment.Id);

            // 4. Trả về DTO
            return MapToCommentResponseDto(created);
        }
    }
}
