using AppointMe.Api.Data;
using AppointMe.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AppointMe.Api.Controllers
{
    [ApiController]
    [Route("api/businesses/{businessId:guid}/services")]
    public class ServicesController : ControllerBase
    {
        private readonly IServiceRepository _services;
        private readonly IBusinessRepository _businesses;

        public ServicesController(IServiceRepository services, IBusinessRepository businesses)
        {
            _services = services;
            _businesses = businesses;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid businessId)
        {
            if(await _businesses.GetByIdAsync(businessId) == null)
            {
                return NotFound();
            }

            var services = await _services.GetAllByBusinessAsync(businessId);

            return Ok(services);
        }

        [HttpGet("{serviceId:guid}")]
        public async Task<IActionResult> GetById(Guid businessId, Guid serviceId)
        {
            var service = await _services.GetByIdAsync(businessId, serviceId);
            return service == null ? NotFound() : Ok(service);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid businessId, Service service)
        {
            if (await _businesses.GetByIdAsync(businessId) == null)
            {
                return NotFound();
            }

            if (service.DurationMinutes % 5 != 0)
            {
                return BadRequest("Duration must be a multiple of 5 minutes.");
            }

            service.BusinessId = businessId;

            var newId = await _services.CreateAsync(service);
            var created = await _services.GetByIdAsync(businessId, newId);

            return CreatedAtAction(nameof(GetById), new { businessId, serviceId = newId }, created);
        }

        [HttpPut("{serviceId:guid}")]
        public async Task<IActionResult> Update(Guid businessId, Guid serviceId, Service service)
        {
            if (service.DurationMinutes % 5 != 0)
            {
                return BadRequest("Duration must be a multiple of 5 minutes.");
            }

            service.Id = serviceId;
            service.BusinessId = businessId;

            var updated = await _services.UpdateAsync(service);
            return updated ? NoContent() : NotFound();
        }

        [HttpDelete("{serviceId:guid}")]
        public async Task<IActionResult> Delete(Guid businessId, Guid serviceId)
        {
            var deleted = await _services.DeleteAsync(businessId, serviceId);
            return deleted ? NoContent() : NotFound();
        }
    }
}