using AppointMe.Api.Auth;
using AppointMe.Api.Models;
using AppointMe.Api.Models.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
namespace AppointMe.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public AuthController(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            Email = request.Email,
            UserName = request.Email,
            FullName = request.FullName
        };
        
        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            // email уже занят - 409
            if (result.Errors.Any(e => e.Code == "DuplicateEmail" || e.Code == "DuplicateUserName"))
            {
                return Conflict("Email is already registered.");
            }

            return BadRequest(result.Errors.Select(e => e.Description));
        }

        await _userManager.AddToRoleAsync(user, request.AccountType);

        return StatusCode(201, new AuthUser
        {
            Id = user.Id,
            Email = user.Email!,
            FullName = user.FullName,
            Roles = new List<string> { request.AccountType }
        });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null || !await _userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized("Invalid email or password.");
        }

        // заблокирован → 403
        if (user.IsBlocked)
        {
            return StatusCode(403, "User is blocked.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var (token, expiresAt) = _tokenService.CreateToken(user, roles);

        return Ok(new AuthResponse
        {
            AccessToken = token,
            ExpiresAt = expiresAt,
            User = new AuthUser
            {
                Id = user.Id,
                Email = user.Email!,
                FullName = user.FullName,
                Roles = roles.ToList()
            }
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new AuthUser
        {
            Id = Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!),
            Email = User.FindFirstValue(JwtRegisteredClaimNames.Email)!,
            FullName = User.FindFirstValue(JwtRegisteredClaimNames.Name)!,
            Roles = User.FindAll("role").Select(c => c.Value).ToList()
        });
    }
}