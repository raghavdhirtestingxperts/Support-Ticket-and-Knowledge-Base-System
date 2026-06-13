namespace SupportTicketAPI.DTOs;


public record CreateTicketDto(
    string Title,
    string Description,
    string Priority   
);

public record AssignTicketDto(int AgentUserId);

public record UpdateStatusDto(
    string NewStatus, 
    string? Note     
);

public record TicketResponseDto(
    int      Id,
    string   Title,
    string   Description,
    string   Status,
    string   Priority,
    DateTime CreatedAt,
    DateTime SlaDeadline,
    bool     IsSlaBreached,
    int      CreatedByUserId,
    string   CreatedByName,
    int?     AssignedToUserId,
    string?  AssignedToName,
    IEnumerable<CommentResponseDto>      Comments,
    IEnumerable<TicketHistoryResponseDto> History
);

public record TicketListItemDto(
    int      Id,
    string   Title,
    string   Status,
    string   Priority,
    DateTime CreatedAt,
    DateTime SlaDeadline,
    bool     IsSlaBreached,
    string   CreatedByName,
    string?  AssignedToName
);

public record TicketHistoryResponseDto(
    int       Id,
    string?   OldStatus,
    string    NewStatus,
    string?   Note,
    DateTime  ChangedAt,
    string    ChangedByName
);
