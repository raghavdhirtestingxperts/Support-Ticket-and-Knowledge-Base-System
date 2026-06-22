using SupportTicketAPI.Models;

namespace SupportTicketAPI.Interfaces;

// data access for dashboard queries
public interface IDashboardRepository
{
    // get all agents
    Task<List<User>> GetAgentsAsync();

    // get all open tickets
    Task<List<Ticket>> GetOpenTicketsAsync();

    // get resolved ticket counts by agent
    Task<Dictionary<int, int>> GetResolvedCountsByAgentAsync();
}
