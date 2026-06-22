namespace SupportTicketAPI.DTOs;

// agent summary (moved from KbDtos for proper separation)
public record AgentSummaryDto(
    int    Id,
    string Name,
    string Email
);
