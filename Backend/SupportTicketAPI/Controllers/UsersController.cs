using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SupportTicketAPI.Constants;
using SupportTicketAPI.DTOs;
using SupportTicketAPI.Interfaces;

namespace SupportTicketAPI.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
[EnableRateLimiting("api")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService) => _userService = userService;

    // get agents list for dropdown
    [HttpGet("agents")]
    [Authorize(Roles = $"{RoleNames.Agent},{RoleNames.Supervisor}")]
    [ProducesResponseType(typeof(IEnumerable<AgentSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAgents()
    {
        var agents = await _userService.GetAgentsAsync();
        return Ok(agents.Select(u => new AgentSummaryDto(u.Id, u.Name, u.Email)));
    }
}
