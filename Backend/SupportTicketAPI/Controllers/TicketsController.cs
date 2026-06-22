using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SupportTicketAPI.Constants;
using SupportTicketAPI.DTOs;
using SupportTicketAPI.Interfaces;
using SupportTicketAPI.Mappers;

namespace SupportTicketAPI.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize]
[EnableRateLimiting("api")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _tickets;

    public TicketsController(ITicketService tickets)
    {
        _tickets = tickets;
    }

    private int    UserId => int.Parse(User.FindFirstValue("UserId")!);
    private string Role   => User.FindFirstValue(ClaimTypes.Role)!;


    [HttpPost]
    [Authorize(Roles = RoleNames.Customer)]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTicketDto dto)
    {
        try
        {

            var ticket = await _tickets.CreateTicketAsync(
                dto.Title, dto.Description, dto.Priority, UserId);

            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, TicketMapper.ToResponse(ticket));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my")]
    [Authorize(Roles = RoleNames.Customer)]
    [ProducesResponseType(typeof(IEnumerable<TicketListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMy()
    {
        var tickets = await _tickets.GetMyTicketsAsync(UserId);
        return Ok(tickets.Select(TicketMapper.ToListItem));
    }

    [HttpGet]
    [Authorize(Roles = $"{RoleNames.Agent},{RoleNames.Supervisor}")]
    [ProducesResponseType(typeof(IEnumerable<TicketListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] string? priority,
        [FromQuery] int?    assignedToUserId)
    {
        var tickets = await _tickets.GetAllTicketsAsync(status, priority, assignedToUserId);
        return Ok(tickets.Select(TicketMapper.ToListItem));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var ticket = await _tickets.GetTicketByIdAsync(id, UserId, Role);
        if (ticket is null) return NotFound(new { message = "Ticket not found or access denied." });
        return Ok(TicketMapper.ToResponse(ticket));
    }

    [HttpGet("breached")]
    [Authorize(Roles = RoleNames.Supervisor)]
    [ProducesResponseType(typeof(IEnumerable<BreachedTicketDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBreached()
    {
        var now     = DateTime.UtcNow;
        var tickets = await _tickets.GetBreachedTicketsAsync();
        return Ok(tickets.Select(t => new BreachedTicketDto(
            t.Id,
            t.Title,
            t.Priority.ToString(),
            t.Status.ToString(),
            t.SlaDeadline,
            Math.Round((now - t.SlaDeadline).TotalHours, 1),
            t.AssignedTo?.Name
        )));
    }

    [HttpPut("{id:int}/assign")]
    [Authorize(Roles = RoleNames.Agent)]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Assign(int id, [FromBody] AssignTicketDto dto)
    {
        try
        {

            var ticket = await _tickets.AssignTicketAsync(id, dto.AgentUserId, UserId);
            if (ticket is null) return NotFound(new { message = "Ticket not found." });
            return Ok(TicketMapper.ToResponse(ticket));
        }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        catch (ArgumentException ex)         { return BadRequest(new { message = ex.Message }); }
    }

    [HttpPut("{id:int}/status")]
    [Authorize(Roles = $"{RoleNames.Agent},{RoleNames.Supervisor}")]
    [ProducesResponseType(typeof(TicketResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        try
        {

            var ticket = await _tickets.UpdateStatusAsync(id, dto.NewStatus, dto.Note, UserId);
            if (ticket is null) return NotFound(new { message = "Ticket not found." });
            return Ok(TicketMapper.ToResponse(ticket));
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

            var comment = await _tickets.AddCommentAsync(id, dto.Content, UserId, Role);
            if (comment is null) return NotFound(new { message = "Ticket not found." });

            return CreatedAtAction(nameof(GetById), new { id },
                new CommentResponseDto(
                    comment.Id,
                    comment.Content,
                    comment.CreatedAt,
                    comment.UserId,
                    comment.User.Name));
        }
        catch (UnauthorizedAccessException ex) { return StatusCode(403, new { message = ex.Message }); }
    }
}
