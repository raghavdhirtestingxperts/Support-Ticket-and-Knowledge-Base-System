using Microsoft.AspNetCore.Mvc;
using SupportTicketAPI.DTOs;
using SupportTicketAPI.Services;

namespace SupportTicketAPI.Controllers;

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
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginAsync(request);

        if (result is null)
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(result);
    }
}
