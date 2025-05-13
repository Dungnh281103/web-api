using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Dtos;
using MyAPI.Interface;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReadingHistoryController : ControllerBase
    {
        private readonly IReadingHistoryRepository _historyRepo;

        public ReadingHistoryController(IReadingHistoryRepository historyRepo)
        {
            _historyRepo = historyRepo;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyHistory()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var list = await _historyRepo.GetByUserAsync(userId);

            // Nếu muốn trả DTO, map ở đây:
            var result = list.Select(r => new ReadingHistoryDto
            {
                StoryId = r.StoryId,
                LastChapterId = r.ChapterId,
                StoryTitle = r.Story?.Title,
                CoverUrl = r.Story.CoverUrl,
                LastChapterNumber = r.ChapterNumber,
                LastReadAt = r.LastReadAt
            }).ToList();

            return Ok(result);
        }



        [HttpGet("story/{storyId}")]
        public async Task<IActionResult> GetLastReadChapter(string storyId)
        {
            // Lấy userId từ JWT
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            // Lấy bản ghi
            var history = await _historyRepo.GetByUserAndStoryAsync(userId, storyId);
            if (history == null)
                return NotFound("Chưa có lịch sử đọc cho truyện này");

            // Map sang DTO
            var dto = new ReadingHistoryDto
            {
                StoryId = history.StoryId,
                LastChapterId = history.ChapterId,
                CoverUrl = history.CoverUrl,
                LastChapterNumber = history.ChapterNumber,
                LastReadAt = history.LastReadAt
            };

            return Ok(dto);
        }


    }

}
