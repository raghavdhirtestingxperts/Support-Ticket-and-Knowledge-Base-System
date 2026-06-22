using SupportTicketAPI.Models;

namespace SupportTicketAPI.Interfaces;

// user data access
public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);

    // get all agents
    Task<List<User>> GetAgentsAsync();

    // add and save users
    Task AddAsync(User user);
    Task SaveAsync();
}

