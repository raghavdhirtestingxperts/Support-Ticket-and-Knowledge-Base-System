using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SupportTicketAPI.DTOs;
using SupportTicketAPI.Interfaces;

namespace SupportTicketAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        // authenticate
        var result = await _authService.LoginAsync(request.Email, request.Password);

        if (result is null)
            return Unauthorized(new { message = "Invalid email or password." });

        // map response
        var (user, token) = result.Value;
        return Ok(new LoginResponseDto(
            Token:  token,
            Email:  user.Email,
            Name:   user.Name,
            Role:   user.Role.ToString(),
            UserId: user.Id
        ));
    }

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var result = await _authService.RegisterAsync(
                request.Name, request.Email, request.Password, request.Role);

            if (result is null)
                return BadRequest(new { message = "Registration failed." });

            var (user, token) = result.Value;
            return Ok(new LoginResponseDto(
                Token:  token,
                Email:  user.Email,
                Name:   user.Name,
                Role:   user.Role.ToString(),
                UserId: user.Id
            ));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

