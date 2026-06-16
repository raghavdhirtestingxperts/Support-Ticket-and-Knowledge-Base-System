using Microsoft.EntityFrameworkCore;
using SupportTicketAPI.Data;
using SupportTicketAPI.DTOs;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Services;

public interface ITicketService
{
    Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto dto, int createdByUserId);
    Task<IEnumerable<TicketListItemDto>> GetMyTicketsAsync(int userId);
    Task<IEnumerable<TicketListItemDto>> GetAllTicketsAsync(string? status, string? priority, int? assignedToUserId);
    Task<TicketResponseDto?> GetTicketByIdAsync(int ticketId, int requestingUserId, string role);
    Task<TicketResponseDto?> AssignTicketAsync(int ticketId, AssignTicketDto dto, int requestingUserId);
    Task<TicketResponseDto?> UpdateStatusAsync(int ticketId, UpdateStatusDto dto, int requestingUserId);
    Task<CommentResponseDto?> AddCommentAsync(int ticketId, AddCommentDto dto, int userId, string role);
    Task<IEnumerable<BreachedTicketDto>> GetBreachedTicketsAsync();
}

public class TicketService : ITicketService
{
    private readonly AppDbContext _db;
    private readonly ISlaService  _sla;

    public TicketService(AppDbContext db, ISlaService sla)
    {
        _db  = db;
        _sla = sla;
    }

    private static readonly Dictionary<TicketStatus, TicketStatus[]> AllowedTransitions = new()
    {
        { TicketStatus.Open,[TicketStatus.InProgress] },
        { TicketStatus.InProgress,[TicketStatus.PendingCustomer, TicketStatus.Resolved] },
        { TicketStatus.PendingCustomer,[TicketStatus.InProgress] },
        { TicketStatus.Resolved,[TicketStatus.Closed] },
        { TicketStatus.Closed,[] }
    };

    public async Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto dto, int createdByUserId)
    {
        if (!Enum.TryParse<TicketPriority>(dto.Priority, ignoreCase: true, out var priority))
            throw new ArgumentException($"Invalid priority '{dto.Priority}'.");

        var now      = DateTime.UtcNow;
        var deadline = _sla.CalculateDeadline(priority, now);

        var ticket = new Ticket
        {
            Title             = dto.Title,
            Description       = dto.Description,
            Status            = TicketStatus.Open,
            Priority          = priority,
            CreatedAt         = now,
            SlaDeadline       = deadline,
            IsSlaBreached     = false,
            CreatedByUserId   = createdByUserId,
            AssignedToUserId  = null
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        return await BuildResponseAsync(ticket.Id);
    }

    public async Task<IEnumerable<TicketListItemDto>> GetMyTicketsAsync(int userId)
    {
        var tickets = await _db.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Where(t => t.CreatedByUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        RefreshBreachFlags(tickets);
        await _db.SaveChangesAsync();

        return tickets.Select(MapToListItem);
    }

    public async Task<IEnumerable<TicketListItemDto>> GetAllTicketsAsync(
        string? status, string? priority, int? assignedToUserId)
    {
        var query = _db.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<TicketStatus>(status.Replace(" ", ""), ignoreCase: true, out var s))
            query = query.Where(t => t.Status == s);

        if (!string.IsNullOrWhiteSpace(priority) &&
            Enum.TryParse<TicketPriority>(priority, ignoreCase: true, out var p))
            query = query.Where(t => t.Priority == p);

        if (assignedToUserId.HasValue)
            query = query.Where(t => t.AssignedToUserId == assignedToUserId.Value);

        var tickets = await query.OrderByDescending(t => t.CreatedAt).ToListAsync();

        RefreshBreachFlags(tickets);
        await _db.SaveChangesAsync();

        return tickets.Select(MapToListItem);
    }

    public async Task<TicketResponseDto?> GetTicketByIdAsync(int ticketId, int requestingUserId, string role)
    {
        var ticket = await _db.Tickets.FindAsync(ticketId);
        if (ticket is null) return null;

        if (role == "Customer" && ticket.CreatedByUserId != requestingUserId)
            return null;

        return await BuildResponseAsync(ticketId);
    }

    public async Task<TicketResponseDto?> AssignTicketAsync(int ticketId, AssignTicketDto dto, int requestingUserId)
    {
        var ticket = await _db.Tickets.FindAsync(ticketId);
        if (ticket is null) return null;
        if (ticket.Status == TicketStatus.Closed)
            throw new InvalidOperationException("Cannot assign a closed ticket.");

        var agent = await _db.Users.FindAsync(dto.AgentUserId);
        if (agent is null || agent.Role != UserRole.Agent)
            throw new ArgumentException("Target user is not a valid agent.");

        ticket.AssignedToUserId = dto.AgentUserId;
        await _db.SaveChangesAsync();

        return await BuildResponseAsync(ticketId);
    }

    public async Task<TicketResponseDto?> UpdateStatusAsync(int ticketId, UpdateStatusDto dto, int requestingUserId)
    {
        if (!Enum.TryParse<TicketStatus>(dto.NewStatus.Replace(" ", ""), ignoreCase: true, out var newStatus))
            throw new ArgumentException($"Invalid status '{dto.NewStatus}'.");

        var ticket = await _db.Tickets.FindAsync(ticketId);
        if (ticket is null) return null;

        if (ticket.Status == TicketStatus.Closed)
            throw new InvalidOperationException("Closed ticket cannot be edited.");

        if (!AllowedTransitions[ticket.Status].Contains(newStatus))
            throw new InvalidOperationException(
                $"Transition from '{ticket.Status}' to '{newStatus}' is not allowed.");

        var oldStatus = ticket.Status;
        ticket.Status = newStatus;

        var history = new TicketHistory
        {
            TicketId         = ticketId,
            OldStatus        = oldStatus,
            NewStatus        = newStatus,
            Note             = dto.Note,
            ChangedAt        = DateTime.UtcNow,
            ChangedByUserId  = requestingUserId
        };
        _db.TicketHistories.Add(history);
        await _db.SaveChangesAsync();

        return await BuildResponseAsync(ticketId);
    }

    public async Task<CommentResponseDto?> AddCommentAsync(int ticketId, AddCommentDto dto, int userId, string role)
    {
        var ticket = await _db.Tickets.FindAsync(ticketId);
        if (ticket is null) return null;

        if (role == "Customer" && ticket.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Customers can only comment on their own tickets.");

        var comment = new TicketComment
        {
            TicketId  = ticketId,
            UserId    = userId,
            Content   = dto.Content,
            CreatedAt = DateTime.UtcNow
        };

        _db.TicketComments.Add(comment);
        await _db.SaveChangesAsync();

        var user = await _db.Users.FindAsync(userId);
        return new CommentResponseDto(comment.Id, comment.Content, comment.CreatedAt, userId, user!.Name);
    }

    public async Task<IEnumerable<BreachedTicketDto>> GetBreachedTicketsAsync()
    {
        var now = DateTime.UtcNow;

        var tickets = await _db.Tickets
            .Include(t => t.AssignedTo)
            .Where(t => t.SlaDeadline < now && t.Status != TicketStatus.Closed)
            .OrderBy(t => t.SlaDeadline)
            .ToListAsync();

        foreach (var t in tickets.Where(t => !t.IsSlaBreached))
            t.IsSlaBreached = true;
        await _db.SaveChangesAsync();

        return tickets.Select(t => new BreachedTicketDto(
            t.Id,
            t.Title,
            t.Priority.ToString(),
            t.Status.ToString(),
            t.SlaDeadline,
            Math.Round((now - t.SlaDeadline).TotalHours, 1),
            t.AssignedTo?.Name
        ));
    }


    private async Task<TicketResponseDto> BuildResponseAsync(int ticketId)
    {
        var t = await _db.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Comments).ThenInclude(c => c.User)
            .Include(t => t.History).ThenInclude(h => h.ChangedBy)
            .FirstAsync(t => t.Id == ticketId);

        return new TicketResponseDto(
            t.Id,
            t.Title,
            t.Description,
            t.Status.ToString(),
            t.Priority.ToString(),
            t.CreatedAt,
            t.SlaDeadline,
            _sla.IsBreached(t.SlaDeadline),
            t.CreatedByUserId,
            t.CreatedBy.Name,
            t.AssignedToUserId,
            t.AssignedTo?.Name,
            t.Comments.OrderBy(c => c.CreatedAt).Select(c => new CommentResponseDto(
                c.Id, c.Content, c.CreatedAt, c.UserId, c.User.Name)),
            t.History.OrderBy(h => h.ChangedAt).Select(h => new TicketHistoryResponseDto(
                h.Id,
                h.OldStatus?.ToString(),
                h.NewStatus.ToString(),
                h.Note,
                h.ChangedAt,
                h.ChangedBy.Name))
        );
    }

    private static TicketListItemDto MapToListItem(Ticket t) => new(
        t.Id, t.Title, t.Status.ToString(), t.Priority.ToString(),
        t.CreatedAt, t.SlaDeadline, t.IsSlaBreached,
        t.CreatedBy.Name, t.AssignedTo?.Name
    );

    private void RefreshBreachFlags(List<Ticket> tickets)
    {
        foreach (var t in tickets.Where(t => !t.IsSlaBreached && _sla.IsBreached(t.SlaDeadline)))
            t.IsSlaBreached = true;
    }
}
