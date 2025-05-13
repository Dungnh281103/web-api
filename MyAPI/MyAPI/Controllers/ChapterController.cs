using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Data;
using MyAPI.Dtos;
using MyAPI.Interface;
using MyAPI.Models;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChapterController : ControllerBase
    {
        private readonly IChapterRepository _chapterRepository;
        private readonly IStoryRepository _storyRepository;
        private readonly IReadingHistoryRepository _historyRepo;

        public ChapterController(
            IChapterRepository chapterRepository,
            IStoryRepository storyRepository,
             IReadingHistoryRepository historyRepo)
        {
            _chapterRepository = chapterRepository;
            _storyRepository = storyRepository;
            _historyRepo = historyRepo;
        }

        // POST: api/chapter/story/{storyId}
        [HttpPost("story/{storyId}")]
        [Authorize]
        public async Task<IActionResult> AddChapter(string storyId, [FromBody] ChapterRequestDto chapterRequest)
        {
            // Kiểm tra xem truyện có tồn tại không
            var story = await _storyRepository.GetByIdAsync(storyId);
            if (story == null)
            {
                return NotFound("Story not found");
            }

            // Tạo chương mới
            var newChapter = new Chapter
            {
                Id = Guid.NewGuid().ToString(),
                StoryId = storyId,
                ChapterNumber = chapterRequest.ChapterNumber,
                Title = chapterRequest.Title,
                Content = chapterRequest.Content,
                Views = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                CommentsCount = 0
            };

            // Lưu chương vào cơ sở dữ liệu
            await _chapterRepository.AddAsync(newChapter);
            await _chapterRepository.SaveChangesAsync();

            var allChapters = await _chapterRepository.GetChaptersByStoryIdAsync(storyId);
            story.TotalChapters = allChapters.Count;

            story.LastChapterUpdate = DateTime.Now;
            await _storyRepository.UpdateAsync(story);
            await _storyRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChapterById), new { id = newChapter.Id }, newChapter);
        }

        // GET: api/chapter/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChapterById(string id)
        {
            var chapter = await _chapterRepository.GetByIdAsync(id);
            if (chapter == null)
            {
                return NotFound("Chapter not found");
            }

            return Ok(chapter);
        }

        [HttpGet("/chapter/{storyId}")]
        public async Task<IActionResult> GetChapterStory(string storyId)
        {
            var chapter = await _chapterRepository.GetChaptersByStoryIdAsync(storyId);
            if (chapter == null)
            {
                return NotFound("Chapter not found");
            }

            return Ok(chapter);
        }

        [HttpGet("latestChapters")]
        public async Task<IActionResult> GetLatestChapters([FromQuery] int top = 10)
        {
            var list = await _chapterRepository.GetLatestChaptersAsync(top);
            return Ok(new { latestChapters = list });
        }

        [HttpDelete("{chapterId}")]
        [Authorize]
        public async Task<IActionResult> DeleteChapter(string chapterId)
        {
            // 1. Lấy chương
            var chapter = await _chapterRepository.GetByIdAsync(chapterId);
            if (chapter == null)
                return NotFound("Chapter not found");

            // 2. Lấy storyId trước khi xóa
            var storyId = chapter.StoryId;

            // 3. Xóa chương
            await _chapterRepository.DeleteAsync(chapterId);
            await _chapterRepository.SaveChangesAsync();

            // 4. Đếm lại tổng chương
            var count = await _chapterRepository.GetChapterCountByStoryIdAsync(storyId);

            // 5. Cập nhật Story.TotalChapters và LastChapterUpdate
            var story = await _storyRepository.GetByIdAsync(storyId);
            if (story != null)
            {
                story.TotalChapters = count;
                story.LastChapterUpdate = DateTime.Now;
                await _storyRepository.UpdateAsync(story);
                await _storyRepository.SaveChangesAsync();
            }

            return NoContent();
        }


        [HttpPost("{id}/view")]
        [Authorize]
        public async Task<IActionResult> IncrementViewAndSaveHistory(string id)
        {
            // 1. Tăng view chương + story
            var counts = await _chapterRepository.UpdateViewsAsync(id);
            if (counts == null)
                return NotFound("Chapter not found");

            // 2. Lấy chapter để có StoryId + ChapterNumber
            var chap = await _chapterRepository.GetByIdAsync(id);
            if (chap == null)
                return NotFound("Chapter not found");

            // 3. Lấy userId (string) từ JWT và parse sang Guid
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (Guid.TryParse(userIdStr, out var userId))
            {
                // 4. Upsert vào ReadingHistory
                await _historyRepo.UpsertAsync(new ReadingHistory
                {
                    UserId = userId,
                    StoryId = chap.StoryId,
                    ChapterId = chap.Id,
                    ChapterNumber = chap.ChapterNumber,
                    LastReadAt = DateTime.Now
                });
            }

            return Ok(new
            {
                chapterViews = counts.ChapterViews,
                storyViews = counts.StoryViews
            });
        }

    }
}
