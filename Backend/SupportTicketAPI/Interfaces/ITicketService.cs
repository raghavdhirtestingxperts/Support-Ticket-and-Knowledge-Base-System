using SupportTicketAPI.Models;

namespace SupportTicketAPI.Interfaces;

// ticket service contract
public interface ITicketService
{
    Task<Ticket> CreateTicketAsync(string title, string description, string priority, int createdByUserId);

    Task<IEnumerable<Ticket>> GetMyTicketsAsync(int userId);

    Task<IEnumerable<Ticket>> GetAllTicketsAsync(string? status, string? priority, int? assignedToUserId);

    // returns ticket if requesting user has access
    Task<Ticket?> GetTicketByIdAsync(int ticketId, int requestingUserId, string role);

    // assign ticket to agent
    Task<Ticket?> AssignTicketAsync(int ticketId, int agentUserId, int requestingUserId);

    // update ticket status
    Task<Ticket?> UpdateStatusAsync(int ticketId, string newStatus, string? note, int requestingUserId);

    // add comment to ticket
    Task<TicketComment?> AddCommentAsync(int ticketId, string content, int userId, string role);

    Task<IEnumerable<Ticket>> GetBreachedTicketsAsync();
}
