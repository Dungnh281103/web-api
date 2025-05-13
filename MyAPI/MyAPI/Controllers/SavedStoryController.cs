using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Dtos;
using MyAPI.Dtos.SavedSotry;
using MyAPI.Dtos.SavedStory;
using MyAPI.Interface;
using System.Security.Claims;

namespace MyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SavedStoryController : ControllerBase
    {
        private readonly ISavedStoryRepository _repo;

        public SavedStoryController(ISavedStoryRepository repo)
        {
            _repo = repo;
        }

        // POST api/SavedStory
        [HttpPost]

        public async Task<ActionResult<SavedStoryResponseDto>> Save([FromBody] SaveStoryDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _repo.SaveAsync(userId, dto.StoryId);
            if (result == null) return BadRequest("Đã lưu trước đó");
            return Ok(result);
        }

        // DELETE api/SavedStory/{storyId}
        [HttpDelete("{storyId}")]
        public async Task<IActionResult> Unsave(string storyId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var ok = await _repo.UnsaveAsync(userId, storyId);
            if (!ok) return NotFound();
            return NoContent();
        }

        // GET api/SavedStory
        [HttpGet]
        public async Task<ActionResult<List<SavedStoryResponseDto>>> GetAll()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var list = await _repo.GetSavedByUserAsync(userId);
            return Ok(list);
        }

        // GET api/SavedStory/is-saved/{storyId}
        [HttpGet("is-saved/{storyId}")]
        public async Task<ActionResult<bool>> IsSaved(string storyId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var flag = await _repo.IsSavedAsync(userId, storyId);
            return Ok(flag);
        }
    }
}
