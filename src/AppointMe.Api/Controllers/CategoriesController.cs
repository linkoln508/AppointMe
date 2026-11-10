using AppointMe.Api.Data;
using AppointMe.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace AppointMe.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _repository;

        public CategoriesController(ICategoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _repository.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _repository.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            try
            {
                var newId = await _repository.CreateAsync(category);
                category.Id = newId;

                // 201
                return CreatedAtAction(nameof(GetById), new { id = newId }, category);
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.UniqueViolation)
            {
                return Conflict($"Category with slug '{category.Slug}' already exists.");
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, Category category)
        {
            // id из адреса главнее, чем id в теле запроса
            category.Id = id;

            var updated = await _repository.UpdateAsync(category);

            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleted = await _repository.DeleteAsync(id);

                return deleted ? NoContent() : NotFound();
            }
            catch (PostgresException ex) when (ex.SqlState == PostgresErrorCodes.ForeignKeyViolation)
            {
                return Conflict("Category is used by businesses and cannot be deleted.");
            }
        }
    }
}
