using Inventory.Core.Abstractions;
using Inventory.Core.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);
        if (response == null)
        {
            return Unauthorized();
        }

        return Ok(response);
    }

    [Authorize]
    [HttpGet("me")]
    public ActionResult Me()
    {
        var email = User.FindFirst("email")?.Value;
        var role = User.FindFirst("role")?.Value;
        return Ok(new { email, role });
    }
}
