using SupportTicketAPI.Models;

namespace SupportTicketAPI.Interfaces;

// user service contract
public interface IUserService
{
    // get all agents
    Task<IEnumerable<User>> GetAgentsAsync();
}
