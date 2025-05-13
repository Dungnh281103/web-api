
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Dtos;
using MyAPI.Interface;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;

        public CommentController(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        // Lấy danh sách comment chính cho 1 story
        [HttpGet("story/{storyId}")]
        public async Task<ActionResult<List<CommentResponseDto>>> GetByStory(string storyId)
        {
            var comments = await _commentRepo.GetCommentsByStoryIdAsync(storyId);
            return Ok(comments);
        }

        // Lấy danh sách comment chính cho 1 chapter
        [HttpGet("chapter/{chapterId}")]
        public async Task<ActionResult<List<CommentResponseDto>>> GetByChapter(string chapterId)
        {
            var comments = await _commentRepo.GetCommentsByChapterIdAsync(chapterId);
            return Ok(comments);
        }

        // Lấy câu trả lời (replies) của 1 comment
        [HttpGet("{commentId}/replies")]
        public async Task<ActionResult<List<CommentResponseDto>>> GetReplies(string commentId)
        {
            var replies = await _commentRepo.GetRepliesAsync(commentId);
            return Ok(replies);
        }

        // Tạo mới comment (cần xác thực)
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<CommentResponseDto>> Create([FromBody] CreateCommentDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var userId = Guid.Parse(userIdClaim);
            var created = await _commentRepo.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(GetReplies), new { commentId = created.Id }, created);
        }

        // Cập nhật số lượt thích
        [HttpPut("{commentId}/likes")]
        public async Task<IActionResult> UpdateLikes(string commentId, [FromQuery] int count)
        {
            await _commentRepo.UpdateLikesCountAsync(commentId, count);
            return NoContent();
        }
    }
}
