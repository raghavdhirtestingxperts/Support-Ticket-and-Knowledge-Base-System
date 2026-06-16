namespace SupportTicketAPI.DTOs;


public record LoginRequestDto(string Email, string Password);

public record LoginResponseDto(
    string Token,
    string Email,
    string Name,
    string Role,
    int UserId
);

