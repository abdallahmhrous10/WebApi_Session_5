using LibrarySystem.Api.Data.Services;
using LibrarySystem.Api.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LibrarySystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    // POST: api/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        return StatusCode(result.StatusCode, result);
    }

    // POST: api/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        return StatusCode(result.StatusCode, result);
    }

    // POST: api/auth/assign-role
    [HttpPost("assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole(
        AssignRoleDto dto)
    {
        var result = await _authService.AssignRole(dto);

        return StatusCode(result.StatusCode, result);
    }

    // GET: api/auth/me
    [HttpGet("me")]
    [Authorize]
    public IActionResult GetCurrentUser()
    {
        var userInfo = new
        {
            UserId = User.FindFirstValue(
                JwtRegisteredClaimNames.Sub),

            UserName = User.FindFirstValue(
                JwtRegisteredClaimNames.UniqueName),

            Email = User.FindFirstValue(
                JwtRegisteredClaimNames.Email),

            Roles = User.FindAll("role")
                .Select(claim => claim.Value)
                .ToList()
        };

        return Ok(userInfo);
    }
}