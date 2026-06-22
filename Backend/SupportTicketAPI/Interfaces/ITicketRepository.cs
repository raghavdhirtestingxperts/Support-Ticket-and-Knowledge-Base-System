using SupportTicketAPI.Models;

namespace SupportTicketAPI.Interfaces;

// ticket data access
public interface ITicketRepository
{
    // find ticket by id
    Task<Ticket?> GetByIdAsync(int id);

    // find ticket by id with details
    Task<Ticket?> GetByIdWithDetailsAsync(int id);

    // get tickets created by user
    Task<List<Ticket>> GetByUserAsync(int userId);

    // get filtered tickets
    Task<List<Ticket>> GetFilteredAsync(TicketStatus? status, TicketPriority? priority, int? assignedToUserId);

    // get breached tickets
    Task<List<Ticket>> GetBreachedAsync();

    // add ticket
    void Add(Ticket ticket);

    // add history record
    void AddHistory(TicketHistory history);

    // add comment
    void AddComment(TicketComment comment);

    // save changes
    Task SaveAsync();
}
