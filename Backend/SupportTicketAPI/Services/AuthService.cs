using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SupportTicketAPI.Interfaces;
using SupportTicketAPI.Models;

namespace SupportTicketAPI.Services;

// handles login, jwt token generation, and account lockout
public class AuthService : IAuthService
{
    private readonly IUserRepository _users;
    private readonly IConfiguration  _config;

    // ── in-memory lockout state ──
    private static readonly ConcurrentDictionary<string, LoginAttemptInfo> _attempts = new();
    private const int    MaxAttempts        = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(5);

    public AuthService(IUserRepository users, IConfiguration config)
    {
        _users  = users;
        _config = config;
    }

    public async Task<(User User, string Token)?> LoginAsync(string email, string password)
    {
        var normalisedEmail = email.Trim().ToLowerInvariant();

        // check if account is currently locked out
        if (IsLockedOut(normalisedEmail))
            return null;

        var user = await _users.FindByEmailAsync(email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            RecordFailedAttempt(normalisedEmail);
            return null;
        }

        // successful login — reset attempts
        _attempts.TryRemove(normalisedEmail, out _);
        return (user, GenerateToken(user));
    }

    public async Task<(User User, string Token)?> RegisterAsync(string name, string email, string password, string role)
    {
        var normalisedEmail = email.Trim().ToLowerInvariant();

        var existingUser = await _users.FindByEmailAsync(normalisedEmail);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("Email address is already registered.");
        }

        if (!Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsedRole))
        {
            throw new ArgumentException($"Invalid role '{role}'.");
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);

        var newUser = new User
        {
            Name         = name.Trim(),
            Email        = normalisedEmail,
            PasswordHash = passwordHash,
            Role         = parsedRole,
            TenantId     = "default",
            CreatedAt    = DateTime.UtcNow
        };

        await _users.AddAsync(newUser);
        await _users.SaveAsync();

        var token = GenerateToken(newUser);
        return (newUser, token);
    }


    // generate jwt for user

    private string GenerateToken(User user)
    {
        var jwtSettings    = _config.GetSection("JwtSettings");
        var secretKey      = jwtSettings["SecretKey"]!;
        var issuer         = jwtSettings["Issuer"]!;
        var audience       = jwtSettings["Audience"]!;
        var expiryMinutes  = int.Parse(jwtSettings["ExpiryInMinutes"]!);

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("UserId",            user.Id.ToString()),
            new Claim("Role",              user.Role.ToString()),
            new Claim("TenantId",          user.TenantId),
            new Claim(ClaimTypes.Name,     user.Name),
            new Claim(ClaimTypes.Email,    user.Email),
            new Claim(ClaimTypes.Role,     user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ── lockout helpers ──

    private static bool IsLockedOut(string email)
    {
        if (!_attempts.TryGetValue(email, out var info))
            return false;

        if (info.LockedUntil.HasValue && DateTime.UtcNow < info.LockedUntil.Value)
            return true;

        // lockout expired — clear
        if (info.LockedUntil.HasValue && DateTime.UtcNow >= info.LockedUntil.Value)
        {
            _attempts.TryRemove(email, out _);
            return false;
        }

        return false;
    }

    private static void RecordFailedAttempt(string email)
    {
        var info = _attempts.GetOrAdd(email, _ => new LoginAttemptInfo());
        info.FailedCount++;

        if (info.FailedCount >= MaxAttempts)
        {
            info.LockedUntil = DateTime.UtcNow.Add(LockoutDuration);
        }
    }

    private class LoginAttemptInfo
    {
        public int FailedCount { get; set; }
        public DateTime? LockedUntil { get; set; }
    }
}
