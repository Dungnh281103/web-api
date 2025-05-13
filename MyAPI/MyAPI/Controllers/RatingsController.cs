using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Data;
using MyAPI.Dtos.Rating;
using MyAPI.Interface;
using System.Security.Claims;

[ApiController]
[Route("api/stories/{storyId}/ratings")]
public class RatingsController : ControllerBase
{
    private readonly IRatingRepository _repo;

    public RatingsController(IRatingRepository repo)
    {
        _repo = repo;
    }

    // 1. Lấy tất cả rating của 1 truyện
    [HttpGet]
    public async Task<IActionResult> GetAll(string storyId)
    {
        var list = await _repo.GetRatingsByStoryIdAsync(storyId);
        return Ok(list);
    }

    // 2. Lấy rating của chính user đang login
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMine(string storyId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var rating = await _repo.GetUserRatingForStoryAsync(storyId, userId);
        if (rating == null) return NotFound();
        return Ok(rating);
    }

    // 3. Tạo mới rating
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(string storyId, RatingCreateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var existing = await _repo.GetUserRatingForStoryAsync(storyId, userId);
        if (existing != null)
            return BadRequest("Bạn đã đánh giá truyện này rồi.");

        var created = await _repo.AddRatingAsync(storyId, userId, dto.Score, dto.Review);
        return CreatedAtAction(nameof(GetMine), new { storyId }, created);
    }

    // 4. Cập nhật rating
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> Update(string storyId, RatingUpdateDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var updated = await _repo.UpdateRatingAsync(storyId, userId, dto.Score, dto.Review);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

}
