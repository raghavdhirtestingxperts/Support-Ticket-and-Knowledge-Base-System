using Microsoft.EntityFrameworkCore;
using SupportTicketAPI.Data;
using SupportTicketAPI.Interfaces;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Repositories;

// dashboard queries implementation
public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _db;

    public DashboardRepository(AppDbContext db) => _db = db;

    public Task<List<User>> GetAgentsAsync()
        => _db.Users
            .Where(u => u.Role == UserRole.Agent)
            .ToListAsync();

    public Task<List<Ticket>> GetOpenTicketsAsync()
        => _db.Tickets
            .Where(t => t.Status != TicketStatus.Closed)
            .ToListAsync();

    public async Task<Dictionary<int, int>> GetResolvedCountsByAgentAsync()
    {
        var groups = await _db.Tickets
            .Where(t => t.Status == TicketStatus.Resolved && t.AssignedToUserId.HasValue)
            .GroupBy(t => t.AssignedToUserId!.Value)
            .Select(g => new { AgentId = g.Key, Count = g.Count() })
            .ToListAsync();

        return groups.ToDictionary(g => g.AgentId, g => g.Count);
    }
}
