using SupportTicketAPI.Models;

namespace SupportTicketAPI.Interfaces;

// authenticates user and generates jwt
public interface IAuthService
{
    Task<(User User, string Token)?> LoginAsync(string email, string password);
    Task<(User User, string Token)?> RegisterAsync(string name, string email, string password, string role);
}

