using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketAPI.DTOs;
using SupportTicketAPI.Services;

namespace SupportTicketAPI.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Supervisor")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboard;

    public DashboardController(IDashboardService dashboard) => _dashboard = dashboard;

    [HttpGet("agent-workload")]
    [ProducesResponseType(typeof(IEnumerable<AgentWorkloadDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAgentWorkload()
    {
        var result = await _dashboard.GetAgentWorkloadAsync();
        return Ok(result);
    }
}
