using AppointMe.Api.Data;
using AppointMe.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointMe.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessesController : ControllerBase
    {
        private readonly IBusinessRepository _businesses;
        private readonly ICategoryRepository _categories;

        // салоны — основная работа, категории - для проверки categoryId
        public BusinessesController(IBusinessRepository businesses, ICategoryRepository categories)
        {
            _businesses = businesses;
            _categories = categories;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var businesses = await _businesses.GetAllAsync();
            return Ok(businesses);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var business = await _businesses.GetByIdWithServicesAsync(id);
            return business == null ? NotFound() : Ok(business);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Business business)
        {

            if (await _categories.GetByIdAsync(business.CategoryId) == null)
            {
                return BadRequest($"Category {business.CategoryId} does not exist.");
            }
            var newId = await _businesses.CreateAsync(business);

            var created = await _businesses.GetByIdAsync(newId);

            return CreatedAtAction(nameof(GetById), new { id = newId }, created);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, Business business)
        {
            if (await _categories.GetByIdAsync(business.CategoryId) == null)
            {
                return BadRequest($"Category {business.CategoryId} does not exist.");
            }

            business.Id = id;
            var updated = await _businesses.UpdateAsync(business);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _businesses.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}