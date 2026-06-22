using SupportTicketAPI.DTOs;

namespace SupportTicketAPI.Interfaces;

// dashboard metrics logic
public interface IDashboardService
{
    Task<IEnumerable<AgentWorkloadDto>> GetAgentWorkloadAsync();
}
