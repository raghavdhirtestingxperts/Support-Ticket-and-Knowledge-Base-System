using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SupportTicketAPI.Constants;
using SupportTicketAPI.DTOs;
using SupportTicketAPI.Interfaces;

namespace SupportTicketAPI.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = RoleNames.Supervisor)]
[EnableRateLimiting("api")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboard;

    public DashboardController(IDashboardService dashboard) => _dashboard = dashboard;

    [HttpGet("agent-workload")]
    [ProducesResponseType(typeof(IEnumerable<AgentWorkloadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAgentWorkload()
    {
        // return workload directly
        var result = await _dashboard.GetAgentWorkloadAsync();
        return Ok(result);
    }
}
