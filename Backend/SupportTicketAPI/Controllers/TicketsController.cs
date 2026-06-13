using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SupportTicketAPI.DTOs;
using SupportTicketAPI.Services;

namespace SupportTicketAPI.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _tickets;

    public TicketsController(ITicketService tickets) => _tickets = tickets;

    private int    UserId => int.Parse(User.FindFirstValue("UserId")!);
    private string Role   => User.FindFirstValue(ClaimTypes.Role)!;

    [HttpPost]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
    {
        try
        {
            var result = await _tickets.CreateTicketAsync(dto, UserId);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my")]
    [Authorize(Roles = "Customer")]
    [ProducesResponseType(typeof(IEnumerable<TicketListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMy()
    {
        var result = await _tickets.GetMyTicketsAsync(UserId);
        return Ok(result);
    }

    [HttpGet]
    [Authorize(Roles = "Agent,Supervisor")]
    [ProducesResponseType(typeof(IEnumerable<TicketListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] int?    assignedToUserId)
    {
        var result = await _tickets.GetAllTicketsAsync(status, priority, assignedToUserId);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _tickets.GetTicketByIdAsync(id, UserId, Role);
        if (result is null) return NotFound(new { message = "Ticket not found or access denied." });
        return Ok(result);
    }

    [HttpGet("breached")]
    [Authorize(Roles = "Supervisor")]
    [ProducesResponseType(typeof(IEnumerable<BreachedTicketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBreached()
    {
        var result = await _tickets.GetBreachedTicketsAsync();
        return Ok(result);
    }

    [HttpPut("{id:int}/assign")]
    [Authorize(Roles = "Agent")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Assign(int id, [FromBody] AssignTicketDto dto)
    {
        try
        {
            var result = await _tickets.AssignTicketAsync(id, dto, UserId);
            if (result is null) return NotFound(new { message = "Ticket not found." });
            return Ok(result);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (ArgumentException ex)         { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Agent,Supervisor")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        try
        {
            var result = await _tickets.UpdateStatusAsync(id, dto, UserId);
            if (result is null) return NotFound(new { message = "Ticket not found." });
            return Ok(result);
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (ArgumentException ex)         { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPost("{id:int}/comments")]
    [ProducesResponseType(typeof(CommentResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddComment(int id, [FromBody] AddCommentDto dto)
    {
        try
        {
            var result = await _tickets.AddCommentAsync(id, dto, UserId, Role);
            if (result is null) return NotFound(new { message = "Ticket not found." });
            return CreatedAtAction(nameof(GetById), new { id }, result);
        }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
    }
}
