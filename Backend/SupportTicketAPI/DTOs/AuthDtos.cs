using System.ComponentModel.DataAnnotations;

namespace SupportTicketAPI.DTOs;


public record LoginRequestDto(
    [Required, EmailAddress, MaxLength(256)]
    string Email,

    [Required, MaxLength(128)]
    string Password
);

public record LoginResponseDto(
    string Token,
    string Email,
    string Name,
    string Role,
    int UserId
);

public record RegisterRequestDto(
    [Required, MaxLength(100)]
    string Name,

    [Required, EmailAddress, MaxLength(256)]
    string Email,

    [Required, MinLength(6), MaxLength(128)]
    string Password,

    [Required, RegularExpression("^(Customer|Agent|Supervisor)$", ErrorMessage = "Role must be Customer, Agent, or Supervisor.")]
    string Role
);


