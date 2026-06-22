using SupportTicketAPI.DTOs;
using SupportTicketAPI.Interfaces;

namespace SupportTicketAPI.Services;

// calculates agent workload metrics
public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repo;

    public DashboardService(IDashboardRepository repo) => _repo = repo;

    public async Task<IEnumerable<AgentWorkloadDto>> GetAgentWorkloadAsync()
    {
        var now            = DateTime.UtcNow;
        var agents         = await _repo.GetAgentsAsync();
        var openTickets    = await _repo.GetOpenTicketsAsync();
        var resolvedCounts = await _repo.GetResolvedCountsByAgentAsync();

        return agents.Select(agent =>
        {
            var agentTickets = openTickets
                .Where(t => t.AssignedToUserId == agent.Id)
                .ToList();

            var openCount     = agentTickets.Count(t =>
                t.Status == Models.TicketStatus.Open        ||
                t.Status == Models.TicketStatus.InProgress  ||
                t.Status == Models.TicketStatus.PendingCustomer);

            var breachedCount = agentTickets.Count(t => t.SlaDeadline < now);
            var resolvedCount = resolvedCounts.GetValueOrDefault((int)agent.Id, 0);

            return new AgentWorkloadDto(agent.Id, agent.Name, openCount, breachedCount, resolvedCount);
        });
    }
}
