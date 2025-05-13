using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAPI.Data;
using MyAPI.Dtos;
using MyAPI.Interface;

namespace MyAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagsController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;

        public TagsController(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }

        // GET: api/tags
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDto>>> GetTags()
        {
            var tags = await _tagRepository.GetAllTagsWithCountAsync();
            return Ok(new { tags });
        }

        // GET: api/tags/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TagDto>> GetTag(string id)
        {
            var tag = await _tagRepository.GetTagWithDetailsAsync(id);

            if (tag == null)
            {
                return NotFound();
            }

            return Ok(new { tag });
        }

        // POST: api/tags
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TagDto>> CreateTag([FromBody] CreateTagDto tagDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tag = new Tag
            {
                Id = Guid.NewGuid().ToString(),
                Name = tagDto.Name,
                StoryCount = 0
            };

            await _tagRepository.AddAsync(tag);
            await _tagRepository.SaveChangesAsync();

            var createdTag = await _tagRepository.GetTagWithDetailsAsync(tag.Id);
            return CreatedAtAction(nameof(GetTag), new { id = tag.Id }, new { tag = createdTag });
        }

        // PUT: api/tags/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTag(string id, [FromBody] UpdateTagDto tagDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            tag.Name = tagDto.Name;

            await _tagRepository.UpdateAsync(tag);
            await _tagRepository.SaveChangesAsync();

            var updatedTag = await _tagRepository.GetTagWithDetailsAsync(id);
            return Ok(new { tag = updatedTag });
        }

        // DELETE: api/tags/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTag(string id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
            {
                return NotFound();
            }

            await _tagRepository.DeleteAsync(id);
            await _tagRepository.SaveChangesAsync();

            return NoContent();
        }
    }

    // DTOs cho tạo và cập nhật Tag
    public class CreateTagDto
    {
        public string Name { get; set; }
    }

    public class UpdateTagDto
    {
        public string Name { get; set; }
    }
}
