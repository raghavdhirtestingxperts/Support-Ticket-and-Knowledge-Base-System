using Microsoft.EntityFrameworkCore;
using SupportTicketAPI.Data;
using SupportTicketAPI.Interfaces;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Repositories;

// user data access
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;

    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> FindByEmailAsync(string email)
        => _db.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> GetByIdAsync(int id)
        => _db.Users.FindAsync(id).AsTask();

    public Task<List<User>> GetAgentsAsync()
        => _db.Users
            .Where(u => u.Role == UserRole.Agent)
            .OrderBy(u => u.Name)
            .ToListAsync();

    public async Task AddAsync(User user)
        => await _db.Users.AddAsync(user);

    public Task SaveAsync()
        => _db.SaveChangesAsync();
}

