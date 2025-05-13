using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyAPI.Data;
using MyAPI.Dtos;
using MyAPI.Interface;

namespace MyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly IStoryRepository _storyRepository;
        private readonly IChapterRepository _chapterRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        


        public StoriesController(
            IStoryRepository storyRepository,
            IChapterRepository chapterRepository,
            ICategoryRepository categoryRepository,
            ITagRepository tagRepository
            )
        {
            _storyRepository = storyRepository;
            _chapterRepository = chapterRepository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            
        }

        //api lay truyen theo danh sac de cu
        [HttpGet("recommended")]
        public async Task<ActionResult<IEnumerable<StoryResponseDto>>> GetRecommendedStories()
        {
            var stories = await _storyRepository.GetRecommendedStoriesAsync();
            return Ok(new { storys = stories });
        }

        [HttpGet("hot")]
        public async Task<ActionResult<List<HotStoryDto>>> GetHotStories([FromQuery] int top = 10)
        {
            var hotStories = await _storyRepository.GetHotStoriesAsync(top);
            return Ok(hotStories);
        }

        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedStories()
        {
            var list = await _storyRepository.GetCompletedStoriesAsync();
            return Ok(list);
        }

        // GET: api/stories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StoryResponseDto>>> GetStories([FromQuery] string title = null)
        {
            var stories = await _storyRepository.GetAllStoriesWithDetailsAsync(title);
            return Ok(new { stories = stories }); // Fixed the property name from "storys" to "stories"
        }

        // GET: api/stories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<StoryResponseDto>> GetStory(string id)
        {
            var story = await _storyRepository.GetStoryWithDetailsAsync(id);

            if (story == null)
            {
                return NotFound();
            }
            var firstChapter = await _chapterRepository.GetFirstChapterUrl(id);
            return Ok(new { story, firstChapter });
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<StoryResponseDto>> CreateStory([FromBody] CreateStoryDto storyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            

            // 1. Tạo Story entity
            var story = new Story
            {
                Id = Guid.NewGuid().ToString(),
                Title = storyDto.Title,
                Author = storyDto.Author,
                Translator = storyDto.Translator,
                CoverUrl = storyDto.CoverUrl,
                Description = storyDto.Description,
                Status = storyDto.Status,
                IsRecommended = storyDto.IsRecommended,
                TotalChapters = 0,
                TotalViews = 0,
                BookmarksCount = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                LastChapterUpdate = DateTime.Now,
                Rating = new RatingSummary { Average = 0, Count = 0 }
            };

            // 2. Lưu Story
            await _storyRepository.AddAsync(story);
            await _storyRepository.SaveChangesAsync();

            // 3. Gắn Categories nếu có
            if (storyDto.CategoryNames?.Any() == true)
            {
                foreach (var name in storyDto.CategoryNames.Distinct())
                {
                    var cat = await _categoryRepository.GetByNameAsync(name);
                    if (cat != null)
                        story.Categories.Add(cat);
                }
                await _storyRepository.SaveChangesAsync();
            }

            // Cập nhật Tag: tương tự
            if (storyDto.TagNames?.Any() == true)
            {
                foreach (var name in storyDto.TagNames.Distinct())
                {
                    var tag = await _tagRepository.GetByNameAsync(name);
                    if (tag != null)
                        story.Tags.Add(tag);
                }
                await _storyRepository.SaveChangesAsync();
            }

            // 5. Trả về dữ liệu đầy đủ
            var created = await _storyRepository.GetStoryWithDetailsAsync(story.Id);
            return CreatedAtAction(nameof(GetStory), new { id = story.Id }, new { story = created });
        }
        // PUT: api/stories/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateStory(string id, [FromBody] UpdateStoryDto storyDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Lấy entity
            var story = await _storyRepository.GetByIdAsync(id);
            if (story == null)
                return NotFound();

            // 2. Cập nhật thuộc tính cơ bản
            story.Title = storyDto.Title ?? story.Title;
            story.Author = storyDto.Author ?? story.Author;
            story.Translator = storyDto.Translator ?? story.Translator;
            story.CoverUrl = storyDto.CoverUrl ?? story.CoverUrl;
            story.Description = storyDto.Description ?? story.Description;
            story.Status = storyDto.Status ?? story.Status;
            story.IsRecommended = storyDto.IsRecommended ?? story.IsRecommended;
            story.UpdatedAt = DateTime.Now;

            await _storyRepository.UpdateAsync(story);
            await _storyRepository.SaveChangesAsync();

            //category
            if (storyDto.CategoryNames?.Any() == true)
            {
                foreach (var name in storyDto.CategoryNames.Distinct())
                {
                    var cat = await _categoryRepository.GetByNameAsync(name);
                    if (cat != null)
                        story.Categories.Add(cat);
                }
                await _storyRepository.SaveChangesAsync();
            }

            // Cập nhật Tag: tương tự
            if (storyDto.TagNames?.Any() == true)
            {
                foreach (var name in storyDto.TagNames.Distinct())
                {
                    var tag = await _tagRepository.GetByNameAsync(name);
                    if (tag != null)
                        story.Tags.Add(tag);
                }
                await _storyRepository.SaveChangesAsync();
            }

            // 5. Trả về
            var updated = await _storyRepository.GetStoryWithDetailsAsync(id);
            return Ok(new { story = updated });
        }

        // DELETE: api/stories/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteStory(string id)
        {
            var story = await _storyRepository.GetByIdAsync(id);
            if (story == null)
            {
                return NotFound();
            }

            await _storyRepository.DeleteAsync(id);
            await _storyRepository.SaveChangesAsync();

            return NoContent();
        }

        //[HttpPost("upload-cover")]
        //public async Task<IActionResult> UploadCoverImage([FromForm] IFormFile image)
        //{
        //    if (image == null || image.Length == 0)
        //        return BadRequest("Image file is required");

        //    var path = await _fileService.UploadAsync(image, "uploads/stories");

        //    // Ví dụ gán cho entity
        //    var story = new Story
        //    {
        //        Title = "Example",
        //        CoverUrl = path // Gán path tương đối vào đây
        //    };

        //    _context.Stories.Add(story);
        //    await _context.SaveChangesAsync();

        //    return Ok(new { coverUrl = path });
        //}


       


    }


}