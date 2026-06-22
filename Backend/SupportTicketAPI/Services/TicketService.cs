using SupportTicketAPI.Constants;
using SupportTicketAPI.Interfaces;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Services;

// implements ticket business logic
public class TicketService : ITicketService
{
    private readonly ITicketRepository _tickets;
    private readonly IUserRepository   _users;
    private readonly ISlaService       _sla;

    public TicketService(ITicketRepository tickets, IUserRepository users, ISlaService sla)
    {
        _tickets = tickets;
        _users   = users;
        _sla     = sla;
    }

    // allowed status transitions
    private static readonly Dictionary<TicketStatus, TicketStatus[]> AllowedTransitions = new()
    {
        { TicketStatus.Open,            [TicketStatus.InProgress] },
        { TicketStatus.InProgress,      [TicketStatus.PendingCustomer, TicketStatus.Resolved] },
        { TicketStatus.PendingCustomer, [TicketStatus.InProgress] },
        { TicketStatus.Resolved,        [TicketStatus.Closed] },
        { TicketStatus.Closed,          [] }
    };


    public async Task<Ticket> CreateTicketAsync(
        string title, string description, string priority, int createdByUserId)
    {
        if (!Enum.TryParse<TicketPriority>(priority, ignoreCase: true, out var parsedPriority))
            throw new ArgumentException($"Invalid priority '{priority}'.");

        var now      = DateTime.UtcNow;
        var deadline = _sla.CalculateDeadline(parsedPriority, now);

        var ticket = new Ticket
        {
            Title            = title,
            Description      = description,
            Status           = TicketStatus.Open,
            Priority         = parsedPriority,
            CreatedAt        = now,
            SlaDeadline      = deadline,
            IsSlaBreached    = false,
            CreatedByUserId  = createdByUserId,
            AssignedToUserId = null
        };

        _tickets.Add(ticket);
        await _tickets.SaveAsync();

        // reload ticket with details
        return (await _tickets.GetByIdWithDetailsAsync(ticket.Id))!;
    }

    public async Task<IEnumerable<Ticket>> GetMyTicketsAsync(int userId)
    {
        var tickets = await _tickets.GetByUserAsync(userId);
        RefreshBreachFlags(tickets);
        await _tickets.SaveAsync();
        return tickets;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync(
        string? status, string? priority, int? assignedToUserId)
    {
        // parse filters
        TicketStatus?   statusEnum   = null;
        TicketPriority? priorityEnum = null;

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<TicketStatus>(status.Replace(" ", ""), ignoreCase: true, out var s))
            statusEnum = s;

        if (!string.IsNullOrWhiteSpace(priority) &&
            Enum.TryParse<TicketPriority>(priority, ignoreCase: true, out var p))
            priorityEnum = p;

        var tickets = await _tickets.GetFilteredAsync(statusEnum, priorityEnum, assignedToUserId);
        RefreshBreachFlags(tickets);
        await _tickets.SaveAsync();
        return tickets;
    }

    public async Task<Ticket?> GetTicketByIdAsync(int ticketId, int requestingUserId, string role)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId);
        if (ticket is null) return null;

        // customer can only view own tickets
        if (role == RoleNames.Customer && ticket.CreatedByUserId != requestingUserId)
            return null;

        if (!ticket.IsSlaBreached && _sla.IsBreached(ticket.SlaDeadline))
        {
            ticket.IsSlaBreached = true;
            await _tickets.SaveAsync();
        }

        return await _tickets.GetByIdWithDetailsAsync(ticketId);
    }

    public async Task<Ticket?> AssignTicketAsync(int ticketId, int agentUserId, int requestingUserId)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId);
        if (ticket is null) return null;

        if (ticket.Status == TicketStatus.Closed)
            throw new InvalidOperationException("Cannot assign a closed ticket.");

        var agent = await _users.GetByIdAsync(agentUserId);
        if (agent is null || agent.Role != UserRole.Agent)
            throw new ArgumentException("Target user is not a valid agent.");

        ticket.AssignedToUserId = agentUserId;
        if (!ticket.IsSlaBreached && _sla.IsBreached(ticket.SlaDeadline))
        {
            ticket.IsSlaBreached = true;
        }
        await _tickets.SaveAsync();

        return await _tickets.GetByIdWithDetailsAsync(ticketId);
    }

    public async Task<Ticket?> UpdateStatusAsync(
        int ticketId, string newStatus, string? note, int requestingUserId)
    {
        if (!Enum.TryParse<TicketStatus>(newStatus.Replace(" ", ""), ignoreCase: true, out var parsedStatus))
            throw new ArgumentException($"Invalid status '{newStatus}'.");

        var ticket = await _tickets.GetByIdAsync(ticketId);
        if (ticket is null) return null;

        if (ticket.Status == TicketStatus.Closed)
            throw new InvalidOperationException("Closed ticket cannot be edited.");

        if (!AllowedTransitions[ticket.Status].Contains(parsedStatus))
            throw new InvalidOperationException(
                $"Transition from '{ticket.Status}' to '{parsedStatus}' is not allowed.");

        var oldStatus = ticket.Status;
        ticket.Status = parsedStatus;

        _tickets.AddHistory(new TicketHistory
        {
            TicketId        = ticketId,
            OldStatus       = oldStatus,
            NewStatus       = parsedStatus,
            Note            = note,
            ChangedAt       = DateTime.UtcNow,
            ChangedByUserId = requestingUserId
        });

        if (!ticket.IsSlaBreached && _sla.IsBreached(ticket.SlaDeadline))
        {
            ticket.IsSlaBreached = true;
        }

        await _tickets.SaveAsync();
        return await _tickets.GetByIdWithDetailsAsync(ticketId);
    }

    public async Task<TicketComment?> AddCommentAsync(
        int ticketId, string content, int userId, string role)
    {
        var ticket = await _tickets.GetByIdAsync(ticketId);
        if (ticket is null) return null;

        if (role == RoleNames.Customer && ticket.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("Customers can only comment on their own tickets.");

        var comment = new TicketComment
        {
            TicketId  = ticketId,
            UserId    = userId,
            Content   = content,
            CreatedAt = DateTime.UtcNow
        };

        _tickets.AddComment(comment);
        await _tickets.SaveAsync();

        // load user reference
        comment.User = (await _users.GetByIdAsync(userId))!;
        return comment;
    }

    public async Task<IEnumerable<Ticket>> GetBreachedTicketsAsync()
    {
        var tickets = await _tickets.GetBreachedAsync();

        foreach (var t in tickets.Where(t => !t.IsSlaBreached))
            t.IsSlaBreached = true;

        await _tickets.SaveAsync();
        return tickets;
    }


    private void RefreshBreachFlags(List<Ticket> tickets)
    {
        foreach (var t in tickets.Where(t => !t.IsSlaBreached && _sla.IsBreached(t.SlaDeadline)))
            t.IsSlaBreached = true;
    }
}
