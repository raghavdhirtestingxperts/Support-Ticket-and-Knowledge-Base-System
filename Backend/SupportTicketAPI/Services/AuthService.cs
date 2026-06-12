using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SupportTicketAPI.Data;
using SupportTicketAPI.DTOs;
using SupportTicketAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace SupportTicketAPI.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<LoginResponseDto?> RegisterAsync(RegisterRequestDto request);
}

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthService(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return new LoginResponseDto(
            Token: GenerateToken(user),
            Email: user.Email,
            Name: user.Name,
            Role: user.Role.ToString(),
            UserId: user.Id
        );
    }

    public async Task<LoginResponseDto?> RegisterAsync(RegisterRequestDto request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            return null; // Email already exists

        if (!Enum.TryParse<UserRole>(request.Role, ignoreCase: true, out var role))
            return null; // Invalid role

        var user = new User
        {
            Name = request.Name,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = role,
            TenantId = "default",
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new LoginResponseDto(
            Token: GenerateToken(user),
            Email: user.Email,
            Name: user.Name,
            Role: user.Role.ToString(),
            UserId: user.Id
        );
    }

    private string GenerateToken(User user)
    {
        var jwtSettings = _config.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"]!;
        var issuer = jwtSettings["Issuer"]!;
        var audience = jwtSettings["Audience"]!;
        var expiryMinutes = int.Parse(jwtSettings["ExpiryInMinutes"]!);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim("UserId",   user.Id.ToString()),
            new Claim("Role",     user.Role.ToString()),
            new Claim("TenantId", user.TenantId),
            new Claim(ClaimTypes.Name,            user.Name),
            new Claim(ClaimTypes.Email,           user.Email),
            new Claim(ClaimTypes.Role,            user.Role.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
