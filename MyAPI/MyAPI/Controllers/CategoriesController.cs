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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoriesController(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _categoryRepository.GetAllCategoriesWithCountAsync();
            return Ok(new { categories });
        }

        // GET: api/categories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(string id)
        {
            var category = await _categoryRepository.GetCategoryWithDetailsAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(new { category });
        }

        // POST: api/categories
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = new Category
            {
                Id = Guid.NewGuid().ToString(),
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            var createdCategory = await _categoryRepository.GetCategoryWithDetailsAsync(category.Id);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, new { category = createdCategory });
        }

        // PUT: api/categories/5
        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(string id, [FromBody] UpdateCategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            category.Name = categoryDto.Name;

            await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();

            var updatedCategory = await _categoryRepository.GetCategoryWithDetailsAsync(id);
            return Ok(new { category = updatedCategory });
        }

        // DELETE: api/categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(string id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            await _categoryRepository.DeleteAsync(id);
            await _categoryRepository.SaveChangesAsync();

            return NoContent();
        }

    }
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class UpdateCategoryDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

}
