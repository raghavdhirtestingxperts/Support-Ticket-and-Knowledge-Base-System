using Microsoft.EntityFrameworkCore;
using SupportTicketAPI.Data;
using SupportTicketAPI.Interfaces;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Repositories;

// ticket queries implementation
public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _db;

    public TicketRepository(AppDbContext db) => _db = db;

    public Task<Ticket?> GetByIdAsync(int id)
        => _db.Tickets.FindAsync(id).AsTask();

    public Task<Ticket?> GetByIdWithDetailsAsync(int id)
        => _db.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Comments).ThenInclude(c => c.User)
            .Include(t => t.History).ThenInclude(h => h.ChangedBy)
            .FirstOrDefaultAsync(t => t.Id == id);

    public Task<List<Ticket>> GetByUserAsync(int userId)
        => _db.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Where(t => t.CreatedByUserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

    public Task<List<Ticket>> GetFilteredAsync(
        TicketStatus? status, TicketPriority? priority, int? assignedToUserId)
    {
        var query = _db.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == status.Value);

        if (priority.HasValue)
            query = query.Where(t => t.Priority == priority.Value);

        if (assignedToUserId.HasValue)
            query = query.Where(t => t.AssignedToUserId == assignedToUserId.Value);

        return query.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    public Task<List<Ticket>> GetBreachedAsync()
    {
        var now = DateTime.UtcNow;
        return _db.Tickets
            .Include(t => t.AssignedTo)
            .Where(t => t.SlaDeadline < now && t.Status != TicketStatus.Closed)
            .OrderBy(t => t.SlaDeadline)
            .ToListAsync();
    }

    public void Add(Ticket ticket)
        => _db.Tickets.Add(ticket);

    public void AddHistory(TicketHistory history)
        => _db.TicketHistories.Add(history);

    public void AddComment(TicketComment comment)
        => _db.TicketComments.Add(comment);

    public Task SaveAsync()
        => _db.SaveChangesAsync();
}
