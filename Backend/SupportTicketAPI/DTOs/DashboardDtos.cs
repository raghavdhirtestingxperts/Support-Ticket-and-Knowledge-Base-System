namespace SupportTicketAPI.DTOs;


public record AgentWorkloadDto(
    int    AgentUserId,
    string AgentName,
    int    OpenTicketCount,
    int    BreachedTicketCount,
    int    ResolvedTicketCount
);

public record BreachedTicketDto(
    int      Id,
    string   Title,
    string   Priority,
    string   Status,
    DateTime SlaDeadline,
    double   HoursOverdue,
    string?  AssignedToName
);
