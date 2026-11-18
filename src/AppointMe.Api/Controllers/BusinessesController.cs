using AppointMe.Api.Data;
using AppointMe.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppointMe.Api.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AppointMe.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BusinessesController : ControllerBase
    {
        private readonly IBusinessRepository _businesses;
        private readonly ICategoryRepository _categories;
        private readonly IAuthorizationService _authorization;

        public BusinessesController(IBusinessRepository businesses, ICategoryRepository categories, IAuthorizationService authorization)
        {
            _businesses = businesses;
            _categories = categories;
            _authorization = authorization;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var businesses = await _businesses.GetAllAsync();
            return Ok(businesses);
        }


        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var business = await _businesses.GetByIdWithServicesAsync(id);
            return business == null ? NotFound() : Ok(business);
        }


        [Authorize(Roles = Roles.BusinessOwner)]
        [HttpPost]
        public async Task<IActionResult> Create(Business business)
        {

            if (await _categories.GetByIdAsync(business.CategoryId) == null)
            {
                return BadRequest($"Category {business.CategoryId} does not exist.");
            }

            business.OwnerId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

            var newId = await _businesses.CreateAsync(business);

            var created = await _businesses.GetByIdAsync(newId);

            return CreatedAtAction(nameof(GetById), new { id = newId }, created);
        }


        [Authorize(Roles = Roles.BusinessOwnerOrAdmin)]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, Business business)
        {
            var existingBusiness = await _businesses.GetByIdAsync(id);

            if (existingBusiness == null)
            {
                return NotFound();
            }

            var result = await _authorization.AuthorizeAsync(User, existingBusiness, "BusinessOwnerOfResource");

            if (!result.Succeeded)
            {
                return Forbid();
            }

            if (await _categories.GetByIdAsync(business.CategoryId) == null)
            {
                return BadRequest($"Category {business.CategoryId} does not exist.");
            }

            business.Id = id;
            var updated = await _businesses.UpdateAsync(business);
            return updated ? NoContent() : NotFound();
        }


        [Authorize(Roles = Roles.BusinessOwnerOrAdmin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var business = await _businesses.GetByIdAsync(id);

            if(business == null)
            {
                return NotFound();
            }

            var result = await _authorization.AuthorizeAsync(User, business, "BusinessOwnerOfResource");

            if (!result.Succeeded)
            {
                return Forbid();
            }

            var deleted = await _businesses.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}