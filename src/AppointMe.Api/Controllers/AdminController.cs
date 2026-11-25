using AppointMe.Api.Auth;
using AppointMe.Api.Models;
using AppointMe.Api.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AppointMe.Api.Data;

namespace AppointMe.Api.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = Roles.Admin)]
public class AdminController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBusinessRepository _businesses;

    public AdminController(UserManager<ApplicationUser> userManager, IBusinessRepository businesses)
    {
        _userManager = userManager;
        _businesses = businesses;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.Email)
            .ToListAsync();

        var result = new List<AdminUserResponse>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            result.Add(new AdminUserResponse
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToList(),
                IsBlocked = user.IsBlocked
            });
        }

        return Ok(result);
    }

    [HttpPost("users/{id:guid}/block")]
    public async Task<IActionResult> BlockUser(Guid id)
    {
        var currentUserId = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

        if (id == currentUserId)
        {
            return BadRequest("Admin cannot block themself.");
        }

        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        user.IsBlocked = true;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    [HttpPost("users/{id:guid}/unblock")]
    public async Task<IActionResult> UnblockUser(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null)
        {
            return NotFound();
        }

        user.IsBlocked = false;
        await _userManager.UpdateAsync(user);

        return NoContent();
    }

    [HttpGet("businesses")]
    public async Task<IActionResult> GetBusinesses()
    {
        return Ok(await _businesses.GetAllForAdminAsync());
    }

    [HttpPost("businesses/{id:guid}/deactivate")]
    public async Task<IActionResult> DeactivateBusiness(Guid id)
    {
        return await _businesses.SetActiveAsync(id, false) ? NoContent() : NotFound();
    }

    [HttpPost("businesses/{id:guid}/activate")]
    public async Task<IActionResult> ActivateBusiness(Guid id)
    {
        return await _businesses.SetActiveAsync(id, true) ? NoContent() : NotFound();
    }
}