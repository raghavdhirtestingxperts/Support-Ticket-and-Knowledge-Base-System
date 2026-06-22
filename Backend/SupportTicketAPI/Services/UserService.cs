using SupportTicketAPI.Interfaces;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Services;

// user service implementation
public class UserService : IUserService
{
    private readonly IUserRepository _users;

    public UserService(IUserRepository users) => _users = users;

    // get all agents
    public async Task<IEnumerable<User>> GetAgentsAsync()
    {
        return await _users.GetAgentsAsync();
    }
}
