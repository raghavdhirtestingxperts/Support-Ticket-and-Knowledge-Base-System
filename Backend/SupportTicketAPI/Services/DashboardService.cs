using Microsoft.EntityFrameworkCore;
using SupportTicketAPI.Data;
using SupportTicketAPI.DTOs;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Services;

public interface IDashboardService
{
    Task<IEnumerable<AgentWorkloadDto>> GetAgentWorkloadAsync();
}

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _db;

    public DashboardService(AppDbContext db) => _db = db;

    public async Task<IEnumerable<AgentWorkloadDto>> GetAgentWorkloadAsync()
    {
        var now = DateTime.UtcNow;

        var agents = await _db.Users
            .Where(u => u.Role == UserRole.Agent)
            .ToListAsync();

        var tickets = await _db.Tickets
            .Where(t => t.Status != TicketStatus.Closed)
            .ToListAsync();

        var resolvedCounts = await _db.Tickets
            .Where(t => t.Status == TicketStatus.Resolved)
            .GroupBy(t => t.AssignedToUserId)
            .Select(g => new { AgentId = g.Key, Count = g.Count() })
            .ToListAsync();

        return agents.Select(agent =>
        {
            var agentTickets= tickets.Where(t => t.AssignedToUserId == agent.Id).ToList();

            var openCount= agentTickets.Count(t => t.Status == TicketStatus.Open || t.Status == TicketStatus.InProgress || t.Status == TicketStatus.PendingCustomer);

            var breachedCount= agentTickets.Count(t => t.SlaDeadline < now);

            var resolvedCount = resolvedCounts.FirstOrDefault(r => r.AgentId == agent.Id)?.Count ?? 0;

            return new AgentWorkloadDto(agent.Id, agent.Name, openCount, breachedCount, resolvedCount);
        });
    }
}
