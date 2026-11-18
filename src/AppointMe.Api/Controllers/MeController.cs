using AppointMe.Api.Auth;
using AppointMe.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AppointMe.Api.Controllers
{
    [ApiController]
    [Route("api/me")]
    public class MeController : ControllerBase
    {
        private readonly IBusinessRepository _businesses;

        public MeController(IBusinessRepository businesses)
        {
            _businesses = businesses;
        }

        [Authorize(Roles = Roles.BusinessOwner)]
        [HttpGet("businesses")]
        public async Task<IActionResult> GetMyBusinesses()
        {
            var userId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

            var businesses = await _businesses.GetByOwnerAsync(userId);

            return Ok(businesses);
        }
    }
}